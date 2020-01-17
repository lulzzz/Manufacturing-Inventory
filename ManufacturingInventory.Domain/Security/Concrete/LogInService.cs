using ManufacturingInventory.Domain.Buisness.Interfaces;

namespace ManufacturingInventory.Domain.Buisness.Concrete {

    public class LogInResponce {
        public IUserService Service { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public LogInResponce(IUserService service, bool success) {
            this.Service = service;
            this.Success = success;
            this.Message = (success && service != null) ? "Success" : "Error logging in";
        }

        public LogInResponce(string message) {
            this.Service = null;
            this.Success = false;
            this.Message = message;
        }

        public LogInResponce(IUserService service) {
            this.Service = service;
            this.Success = (service != null);
            this.Message = (this.Success) ? "Success" : "Login Failed";
        }
    }

    public class LogInService : ILogInService {
        public static readonly string DefaultDomainPermissions = "InventoryUserLimitedAccount";

        private IDomainManager _domainOperations;
        private IUserServiceProvider _userServiceProvider;

        public LogInService(IDomainManager domainmanagment, IUserServiceProvider userServiceProvider) {
            this._userServiceProvider = userServiceProvider;
            this._domainOperations = domainmanagment;
        }

        public LogInResponce LogInWithPassword(string username, string password, bool storePassword) {
            try {

                var result = this._domainOperations.Authenticate(username, password);
                switch (result.Status) {
                    case AuthenticationStatus.Authenticated: {
                        var user = ((AuthenticatedResult)result).User;
                        string permission = ((AuthenticatedResult)result).Permission;
                        var service = this._userServiceProvider.CreateUserServiceUserAuthenticated(user, storePassword, permission, password);
                        return new LogInResponce(service);
                    }
                    case AuthenticationStatus.InvalidCredentials: {
                        return new LogInResponce("Invalid Credentials,please try logging in again without saved password");
                    }

                    case AuthenticationStatus.UserNotFound: {
                        return new LogInResponce("User was no found on domain, Please contact administrator");
                    }

                    case AuthenticationStatus.NoPermissions: {
                        var user = ((NoPermissionsResult)result).User;
                        var service = this._userServiceProvider.CreateUserServiceNoPermissions(user, storePassword, password);
                        return new LogInResponce(service);
                    }
                    default: {
                        return new LogInResponce("Internal Error, Please contact administrator");
                    }
                }
            } catch (LDAPCommunicationException e) {
                throw e;
            }
        }
    }
}
