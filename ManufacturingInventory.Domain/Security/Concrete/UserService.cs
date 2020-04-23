using Castle.Core.Internal;
using ManufacturingInventory.Domain.Security.Interfaces;
using System.Collections.Generic;

namespace ManufacturingInventory.Domain.Buisness.Concrete {
    public class UserService : IUserService {
        public string CurrentUserName { get; set; }
        public string UserPermissionName { get; set; }
        public int? CurrentSessionId { get; set; }
        public List<UserAction> AvailableUserActions { get; private set; }


        public UserService(string currentUserName, int currentSessionId, string userPermissionName,List<UserAction> userActions) {
            this.CurrentUserName = currentUserName;
            this.CurrentSessionId = currentSessionId;
            this.UserPermissionName = userPermissionName;
            this.AvailableUserActions = userActions;
        }

        public UserService(){
            this.CurrentUserName = null;
            this.CurrentSessionId = null;
            this.UserPermissionName = null;
        }

        public bool IsValid(){
            return (!this.CurrentUserName.IsNullOrEmpty() && this.CurrentSessionId.HasValue);
        }

        public bool Validate(UserAction action) {
            return this.AvailableUserActions.Contains(action);
        }
    }
}
