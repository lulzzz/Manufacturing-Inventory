using System.Collections.Generic;

namespace ManufacturingInventory.Domain.Security.Interfaces {
    public enum UserAction {
        Add,
        Edit,
        Remove,
        CheckIn,
        CheckOut,
        UserManagement
    }

    public interface IUserService {
        string CurrentUserName { get; set; }
        int? CurrentSessionId { get; set; }
        string UserPermissionName { get; set; }
        List<UserAction> AvailableUserActions { get; }
        bool Validate(UserAction action);
        bool IsValid();
    }
}
