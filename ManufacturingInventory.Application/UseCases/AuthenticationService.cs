using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.Authentication;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Security.Concrete;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Serilog;

namespace ManufacturingInventory.Application.UseCases {
    public class AuthenticationService : IAuthenticationUseCase {
        private ManufacturingContext _context;
        private IRepository<User> _userRepository;
        private IRepository<Session> _sessionRepository;
        private IRepository<Permission> _permissionRepository;
        private IUnitOfWork _unitOfWork;
        private IDomainManager _domainManager;
        private ILogger _logger;
        private IUserSettingsService _userSettingsService;


        public AuthenticationService(ManufacturingContext context,IDomainManager domainManager,ILogger logger, IUserSettingsService userSettingsService) {
            this._context = context;
            this._userRepository = new UserRepository(this._context);
            this._sessionRepository = new SessionRepository(this._context);
            this._permissionRepository = new PermissionRepository(this._context);
            this._unitOfWork = new UnitOfWork(this._context);
            this._domainManager = domainManager;
            this._logger = logger;
            this._userSettingsService = userSettingsService;
        }

        public async Task<AuthenticationOutput> Execute(AuthenticationInput input) {
            switch (input.Action) {
                case AuthenticationActions.LOGIN:
                    return await this.ExecuteLogIn(input);
                case AuthenticationActions.LOGOUT:
                    return await this.ExecuteLogOut(input);
                default:
                    return new AuthenticationOutput(null, false, "Error: Invalid User Action");
            }
        }

        private async Task<AuthenticationOutput> ExecuteLogIn(AuthenticationInput input) {
            AuthenticationResult result;
            try {
                result = this._domainManager.Authenticate(input.UserName, input.Password);
            } catch(LDAPCommunicationException e) {
                this._logger.Error(e, "Could not connect to domain");
                return new AuthenticationOutput(null, false, "Authentication Error: Could not connect to domain");
            }
            switch (result.Status) {
                case AuthenticationStatus.Authenticated: {
                    var user = ((AuthenticatedResult)result).User;
                    string permission = ((AuthenticatedResult)result).Permission;
                    var entity = await this._userRepository.GetEntityAsync(e => e.UserName == user.UserName);
                    IUserService userService;
                    if (entity != null) {
                        userService=await this.CreateUserServiceExisitingUser(entity, input.SaveCredentials, permission, input.Password);
                    } else {
                        userService=await this.CreateUserServiceNewUser(user, input.SaveCredentials, permission, input.Password);
                    }
                    if (userService != null) {
                        return new AuthenticationOutput(userService, true, "Success:  User logged in");
                    } else {
                        return new AuthenticationOutput(null, false, "Error: Login Failed, Please contact admin");
                    }
                }
                case AuthenticationStatus.NoPermissions:
                    return new AuthenticationOutput(null, false, "Error: Invalid Credentials, please check input and try again");
                case AuthenticationStatus.InvalidCredentials:
                    return new AuthenticationOutput(null,false,"Error: Invalid Credentials, please check input and try again");
                case AuthenticationStatus.UserNotFound:
                    return new AuthenticationOutput(null, false, "Error: User Not Found, Please contact admin");
                default:
                    return new AuthenticationOutput(null, false, "Internal Error: Invalid Auth Status, Please contact admin");
            }
        }

        private async Task<AuthenticationOutput> ExecuteLogOut(AuthenticationInput input) {
            if (input.UserService.CurrentSessionId.HasValue) {
                var session = await this._sessionRepository.GetEntityAsync(e => e.Id == input.UserService.CurrentSessionId);
                if (session != null) {
                    session.Out = DateTime.Now;
                    var updated = await this._sessionRepository.UpdateAsync(session);
                    if (updated != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            return new AuthenticationOutput(null, true, "Succesfully Logged Out");
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error:  Could not log out,Session Save Failed");
                            return new AuthenticationOutput(null, false, "Error:  Could not log out, Session Save Failed.  Please Contact Admin");
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error:  Could not log out, Session Update Failed");
                        return new AuthenticationOutput(null, false, "Error:  Could not log out, Session Update Failed.  Please Contact Admin");
                    }
                } else {
                    this._logger.Error("Error:  Could not log out, Current Session Not Found.");
                    return new AuthenticationOutput(null, false, "Error:  Could not log out, Current Session Not Found.  Please Contact Admin");
                }
            }
            return new AuthenticationOutput(null, false, "Not Implemented Yet");
        }

        private async Task<IUserService> CreateUserServiceNewUserDefault(User userEntity,bool storePassword, string password = null) {
            var permissionEnity = await this._permissionRepository.GetEntityAsync(e => e.Name == "InventoryUserAccount");
            if (permissionEnity != null) {
                userEntity.Permission = permissionEnity;
                if (storePassword && !string.IsNullOrEmpty(password)) {
                    try {
                        var result = await EncryptionService.Encrypt(password);
                        userEntity.EncryptedPassword = Convert.ToBase64String(result.Encrypted);
                        userEntity.IV = result.IV;
                        userEntity.Key = result.Key;
                        try {
                            var userSettings = new UserSettings() {
                                UserName = userEntity.UserName,
                                Password = userEntity.EncryptedPassword,
                                IV = Convert.ToBase64String(userEntity.IV),
                                Key = Convert.ToBase64String(userEntity.Key)
                            };
                            await this._userSettingsService.SaveUserSettings(userSettings);
                        } catch {
                            this._logger.Error("Error writing out user settings");
                        }
                    } catch (Exception e) {
                        this._logger.Error(e, "Error Encrypting Password");
                        return null;
                    }
                }
                var addedSession = await this._sessionRepository.AddAsync(new Session(userEntity));
                if (addedSession != null) {
                    var updated = await this._userRepository.AddAsync(userEntity);
                    if (updated != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            var userActions = this.GenerateAvailableActions(userEntity.Permission.Name);
                            return new UserService(userEntity.Id,userEntity.UserName, addedSession.Id, userEntity.Permission.Name,userActions);
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error: Could not save, AuthUseCase->CreateUserServiceNewUser");
                            return null;
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error: Could not update user, AuthUseCase->CreateUserServiceNewUser");
                        return null;
                    }
                } else {
                    await this._unitOfWork.Undo();
                    this._logger.Error("Error: Could not add session, AuthUseCase->CreateUserServiceNewUser");
                    return null;
                }
            } else {
                this._logger.Error("Error: Could not find default permission AuthUseCase->CreateUserServiceNewUser");
                return null;
            }
        }

        private async Task<IUserService> CreateUserServiceExisitingUser(User entity, bool storePassword, string permission, string password = null) {
            if (entity.Permission.Name == permission) {
                if (storePassword && !string.IsNullOrEmpty(password)) {
                    try {
                        var result = await EncryptionService.Encrypt(password);
                        entity.EncryptedPassword = Convert.ToBase64String(result.Encrypted);
                        entity.IV = result.IV;
                        entity.Key = result.Key;
                        try {
                            var userSettings = new UserSettings() {
                                UserName = entity.UserName,
                                Password = entity.EncryptedPassword,
                                IV = Convert.ToBase64String(entity.IV),
                                Key = Convert.ToBase64String(entity.Key)
                            };
                            await this._userSettingsService.SaveUserSettings(userSettings);
                        } catch {
                            this._logger.Error("Error writing out user settings");
                        }
                    } catch (Exception e) {
                        this._logger.Error(e, "Error Encrypting Password");
                        return null;
                    }
                }
                var addedSession = await this._sessionRepository.AddAsync(new Session(entity));
                if (addedSession != null) {
                    var updated = await this._userRepository.UpdateAsync(entity);
                    if (updated != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            var userActions = this.GenerateAvailableActions(entity.Permission.Name);
                            return new UserService(entity.Id,entity.UserName, addedSession.Id, entity.Permission.Name,userActions);
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error: Could not save, AuthUseCase->CreateUserServiceExisitingUser->DB permissions are same as domain");
                            return null;
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error: Could not update user, AuthUseCase->CreateUserServiceExisitingUser-> DB permissions are same as domain");
                        return null;
                    }
                } else {
                    await this._unitOfWork.Undo();
                    this._logger.Error("Error: Could not add session, AuthUseCase->CreateUserServiceExisitingUser-> DB permissions are same as domain");
                    return null;
                }
            } else {//DB permissions do not match domain, Domain overrides
                var permissionEnity = await this._permissionRepository.GetEntityAsync(e => e.Name == permission);
                if (permissionEnity != null) {
                    entity.Permission = permissionEnity;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        try {
                            var result = await EncryptionService.Encrypt(password);
                            entity.EncryptedPassword = Convert.ToBase64String(result.Encrypted);
                            entity.IV = result.IV;
                            entity.Key = result.Key;
                            try {
                                var userSettings = new UserSettings() {
                                    UserName = entity.UserName,
                                    Password = entity.EncryptedPassword,
                                    IV = Convert.ToBase64String(entity.IV),
                                    Key = Convert.ToBase64String(entity.Key)
                                };
                                await this._userSettingsService.SaveUserSettings(userSettings);
                            } catch {
                                this._logger.Error("Error writing out user settings");
                            }
                        } catch (Exception e) {
                            this._logger.Error(e, "Error Encrypting Password");
                            return null;
                        }
                    }

                    var addedSession = await this._sessionRepository.AddAsync(new Session(entity));
                    if (addedSession != null) {
                        var updated = await this._userRepository.UpdateAsync(entity);
                        if (updated != null) {
                            var count = await this._unitOfWork.Save();
                            if (count > 0) {
                                var userActions = this.GenerateAvailableActions(entity.Permission.Name);
                                return new UserService(entity.Id,entity.UserName, addedSession.Id, entity.Permission.Name,userActions);
                            } else {
                                await this._unitOfWork.Undo();
                                this._logger.Error("Error: Could not save, AuthUseCase->CreateUserService-> DB permissions do not match domain, Domain overrides");
                                return null;
                            }
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error: Could not update user, AuthUseCase->CreateUserService-> DB permissions do not match domain, Domain overrides");
                            return null;
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error: Could not add session, AuthUseCase->CreateUserService-> DB permissions do not match domain, Domain overrides");
                        return null;
                    }
                } else {
                    this._logger.Error("Error: Could not find default permission AuthUseCase->CreateUserService-> DB permissions do not match domain, Domain overrides");
                    return null;
                }
            }
        }
        
        private async Task<IUserService> CreateUserServiceNewUser(User entity, bool storePassword, string permission = null, string password = null) {
            if (!string.IsNullOrEmpty(permission)) {
                var permissionEnity = await this._permissionRepository.GetEntityAsync(e => e.Name == permission);
                if (permissionEnity != null) {
                    entity.Permission = permissionEnity;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        try {
                            var result = await EncryptionService.Encrypt(password);
                            entity.EncryptedPassword = Convert.ToBase64String(result.Encrypted);
                            entity.IV = result.IV;
                            entity.Key = result.Key;
                            try {
                                var userSettings = new UserSettings() {
                                    UserName = entity.UserName,
                                    Password = entity.EncryptedPassword,
                                    IV = Convert.ToBase64String(entity.IV),
                                    Key = Convert.ToBase64String(entity.Key)
                                };
                                await this._userSettingsService.SaveUserSettings(userSettings);
                            } catch {
                                this._logger.Error("Error: writing out user settings, AuthUseCase->CreateUserServiceNewUser");
                            }
                        } catch (Exception e) {
                            this._logger.Error(e, "Error: Encrypting Password, AuthUseCase->CreateUserServiceNewUser");
                            return null;
                        }
                    }
                    var addedSession = await this._sessionRepository.AddAsync(new Session(entity));
                    if (addedSession != null) {
                        var updated = await this._userRepository.AddAsync(entity);
                        if (updated != null) {
                            var count = await this._unitOfWork.Save();
                            if (count > 0) {
                                var userActions= this.GenerateAvailableActions(entity.Permission.Name);
                                return new UserService(entity.Id,entity.UserName, addedSession.Id, entity.Permission.Name, userActions);
                            } else {
                                await this._unitOfWork.Undo();
                                this._logger.Error("Error: Could not save, AuthUseCase->CreateUserServiceNewUser");
                                return null;
                            }
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error: Could not update user, AuthUseCase->CreateUserServiceNewUser");
                            return null;
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error: Could not add session, AuthUseCase->CreateUserServiceNewUser");
                        return null;
                    }
                } else {
                    this._logger.Error("Error: Could not find default permission AuthUseCase->CreateUserServiceNewUser");
                    return null;
                }
            } else {//No permissions.  Assign user default(InventoryViewOnly) permissions
                return await this.CreateUserServiceNewUserDefault(entity, storePassword, password);
            }
        }

        private List<UserAction> GenerateAvailableActions(string permissionName) {
            List<UserAction> availableActions = new List<UserAction>();
            if (permissionName == "InventoryAdminAccount") {
                availableActions.Add(UserAction.Add);
                availableActions.Add(UserAction.Edit);
                availableActions.Add(UserAction.Remove);
                availableActions.Add(UserAction.CheckIn);
                availableActions.Add(UserAction.CheckOut);
                availableActions.Add(UserAction.UserManagement);

            } else if (permissionName == "InventoryUserLimitedAccount") {
                availableActions.Add(UserAction.CheckIn);
                availableActions.Add(UserAction.CheckOut);

            } else if (permissionName == "InventoryUserFullAccount") {
                availableActions.Add(UserAction.Add);
                availableActions.Add(UserAction.Edit);
                availableActions.Add(UserAction.Remove);
                availableActions.Add(UserAction.CheckIn);
                availableActions.Add(UserAction.CheckOut);
            }
            return availableActions;
        }
        
        public async Task<IEnumerable<User>> GetUsers() {
            return await this._userRepository.GetEntityListAsync();
        }

        public async Task<User> GetUser(string userName) {
            return await this._userRepository.GetEntityAsync(e => e.UserName == userName);
        }
    }
}
