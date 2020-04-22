using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace ManufacturingInventory.Domain.Security.Interfaces {

    public interface IDomainManager {
        IDictionary<string, string> InventoryGroups { get; set; }
        bool AddUserToGroup(string user, string groupName);
        AuthenticationResult Authenticate(string username, string password);
        IList<string> GetGroupMembers(string groupSamName);
        IList<User> GetSETUsers();
        Permission GetUserInventoryPermission(UserPrincipal user);
        bool RemoveUserFromGroup(string user, string groupName);
        User GetDomainUser(string userName);
        IList<string> AllUserInventoryPermsions(string username);
        bool RemoveAllUserInventoryPermissions(string username);
    }
}