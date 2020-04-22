using System;
using System.Collections.Generic;
using System.Text;
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
    public class AuthenticationUseCase : IAuthenticationUseCase {
        private ManufacturingContext _context;
        private IRepository<User> _userRepository;
        private IRepository<Session> _sessionRepository;
        private IRepository<Permission> _permissionRepository;
        private IUnitOfWork _unitOfWork;
        private IDomainManager _domainManager;
        private ILogger _logger;
        private IUserSettingsService _userSettingsService;


        public AuthenticationUseCase(ManufacturingContext context,IDomainManager domainManager,ILogger logger, IUserSettingsService userSettingsService) {
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
                case AuthenticationActions.UPDATE:
                    return await this.ExecuteUpdate(input);
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

        private async Task<IUserService> CreateUserServiceExisitingUser(User entity,bool storePassword,string permission,string password=null) {
            if (entity.Permission.Name == permission) {
                if (storePassword && !string.IsNullOrEmpty(password)) {
                    try {
                        var result = await EncryptionService.Encrypt(password);
                        entity.EncryptedPassword =Convert.ToBase64String(result.Encrypted);
                        entity.IV = result.IV;
                        entity.Key = result.Key;
                        try {
                            var userSettings = new UserSettings() {
                                UserName = entity.UserName,
                                Password = entity.EncryptedPassword,
                                IV = Convert.ToBase64String(entity.IV),
                                Key= Convert.ToBase64String(entity.Key)
                            };
                            await this._userSettingsService.SaveUserSettings(userSettings);
                        } catch {
                            this._logger.Error("Error writing out user settings");
                        }
                    } catch(Exception e) {
                        this._logger.Error(e, "Error Encrypting Password");
                        return null;
                    }
                }
                var addedSession=await this._sessionRepository.AddAsync(new Session(entity));
                if (addedSession != null) {
                    var updated = await this._userRepository.UpdateAsync(entity);
                    if (updated != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            return new UserService(entity, addedSession, entity.Permission);
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error: Could not save, AuthUseCase->CreateUserService-> First Case");
                            return null;
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error: Could not update user, AuthUseCase->CreateUserService-> First Case");
                        return null;
                    }
                } else {
                    await this._unitOfWork.Undo();
                    this._logger.Error("Error: Could not add session, AuthUseCase->CreateUserService-> First Case");
                    return null;
                }
            } else {
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
                                return new UserService(entity, addedSession, entity.Permission);
                            } else {
                                await this._unitOfWork.Undo();
                                this._logger.Error("Error: Could not save, AuthUseCase->CreateUserService-> Second Case");
                                return null;
                            }
                        } else {
                            await this._unitOfWork.Undo();
                            this._logger.Error("Error: Could not update user, AuthUseCase->CreateUserService-> Second Case");
                            return null;
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        this._logger.Error("Error: Could not add session, AuthUseCase->CreateUserService-> Second Case");
                        return null;
                    }
                } else {
                    this._logger.Error("Error: Could not find default permission AuthUseCase->CreateUserService-> Second Case");
                    return null;
                }
            }
        }

        private async Task<IUserService> CreateUserServiceNewUser(User entity, bool storePassword, string permission=null, string password=null) {
            if (!string.IsNullOrEmpty(permission)) {
                var permissionEnity = await this._permissionRepository.GetEntityAsync(e => e.Name==permission);
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
                                return new UserService(entity, addedSession, entity.Permission);
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
            } else {
                var permissionEnity = await this._permissionRepository.GetEntityAsync(e => e.Name == "InventoryUserAccount");
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
                                return new UserService(entity, addedSession, entity.Permission);
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
        }

        private async Task<AuthenticationOutput> ExecuteLogOut(AuthenticationInput input) {
            return new AuthenticationOutput(null, false, "Not Implemented Yet");
        }

        private async Task<AuthenticationOutput> ExecuteCreate(AuthenticationInput input) {
            return new AuthenticationOutput(null, false, "Not Implemented Yet");
        }

        private async Task<AuthenticationOutput> ExecuteDelete(AuthenticationInput input) {
            return new AuthenticationOutput(null, false, "Not Implemented Yet");
        }

        public async Task<AuthenticationOutput> ExecuteUpdate(AuthenticationInput input) {
            return new AuthenticationOutput(null, false, "Not Implemented Yet");
        }


        public Task<Session> GetSession(int sessionId) => throw new NotImplementedException();
        public Task<User> GetUser(int userId) => throw new NotImplementedException();
        public Task<IEnumerable<User>> GetUsers() => throw new NotImplementedException();
        public Task<IEnumerable<Session>> GetUserSession(int userId) => throw new NotImplementedException();
        public Task<User> GetUser(string userName) => throw new NotImplementedException();
    }
}
