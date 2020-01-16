using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Effortless.Net.Encryption;
using System.Text;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Domain.Buisness.Concrete {

    public enum AuthenticationStatus{
        InvalidCredentials,
        UserNotFound,
        NoPermissions,
        Authenticated
    }

    public abstract class AuthenticationResult {
        public AuthenticationStatus Status { get; private set; }
        public AuthenticationResult(AuthenticationStatus status)
        {
            this.Status=status;
        }
    }

    public class InvalidCredentialsResult : AuthenticationResult {
        public InvalidCredentialsResult() : base(AuthenticationStatus.InvalidCredentials) { }
    }

    public class UserNotFoundResult : AuthenticationResult {
        public UserNotFoundResult() : base(AuthenticationStatus.UserNotFound) { }
    }

    public class NoPermissionsResult:AuthenticationResult {
        public User User { get; private set; }
      
        public NoPermissionsResult(User user) : base(AuthenticationStatus.NoPermissions) {
            this.User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }

    public class AuthenticatedResult:AuthenticationResult  {
        public User User { get; private set; }
        public string Permission { get; private set; }

        public AuthenticatedResult(User user,string userPermission):base(AuthenticationStatus.Authenticated) {
            this.User = user ?? throw new ArgumentNullException(nameof(user));
            this.Permission = userPermission ?? throw new ArgumentNullException(nameof(userPermission));
        }
    }

    public class LDAPCommunicationException : Exception {
        public LDAPCommunicationException()
        {
        }

        public LDAPCommunicationException(string message)
            : base(message)
        {
        }

        public LDAPCommunicationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DomainManager : IDomainManager {
        public static readonly string SAMUSERROOT = "SETI-W-AllUsers";
        public static readonly string InventoryContainer = "OU=Inventory,OU=Program Access,OU=Groups,OU=SETI,DC=seti,DC=com";
        public static readonly string SetiUserContainer = "OU=SETI Users,DC=seti,DC=com";
        private string username;
        private string password;
        private string address;


        private IDictionary<string, string> _inventoryGroups;

        public IDictionary<string, string> InventoryGroups {
            get => this._inventoryGroups;
            set => this._inventoryGroups = value;
        }

        //public DomainManager()
        //{
        //    this.GetLDAPCredentials();
        //}

        public DomainManager() {

        }

        private void GetLDAPCredentials()
        {
            //this.username=Properties.Settings.Default.LDAP_UserName;
            //var encryptedPass = Properties.Settings.Default.LDAP_EncryptedPassword;
            //var encryptedAddr = Properties.Settings.Default.LDAP_EncryptedAddress;
            //var key =Convert.FromBase64String(Properties.Settings.Default.LDAP_Key);
            //var iv= Convert.FromBase64String(Properties.Settings.Default.LDAP_IV);
            //this.password = Strings.Decrypt(encryptedPass, key, iv);
            //this.address = Strings.Decrypt(encryptedAddr, key, iv);
            //StringBuilder message = new StringBuilder();
            //message.AppendFormat("Username: {0} Password: {1} Address: {2}", this.username, this.password, this.address);
        }

        public AuthenticationResult Authenticate(string username, string password)
        {
            try {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, this.address, this.username, this.password)) {
                    UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
                    if (user != null) {
                        if (context.ValidateCredentials(username, password)) {
                            User newUser = new User() {
                                Email = user.EmailAddress,
                                FirstName = user.GivenName,
                                LastName = user.Surname,
                                Extension = user.VoiceTelephoneNumber,
                                UserName = username
                            };
                            var permission = this.GetUserInventoryPermission(user);
                            if (permission!=null) {
                                return new AuthenticatedResult(newUser, permission.Name);
                            } else {
                                return new NoPermissionsResult(newUser);
                            }
                        } else {
                            return new InvalidCredentialsResult();
                        }
                    } else {
                        return new UserNotFoundResult();
                    }
                }
            }catch(Exception e) { 
                throw new LDAPCommunicationException("Internal Error: Error initializing LDAP Context, Please contact administrator..",e);
            }
        }

        public IList<string> AllUserInventoryPermsions(string username) {
            List<string> userGroups = new List<string>();
            try {
                using(PrincipalContext pc = new PrincipalContext(ContextType.Domain, this.address, this.username, this.password)) {
                    UserPrincipal userprinciple = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, username);
                    using(PrincipalContext inventoryContext = new PrincipalContext(ContextType.Domain, this.address, DomainManager.InventoryContainer, this.username, this.password)) {
                        GroupPrincipal findAllGroups = new GroupPrincipal(inventoryContext, "*");
                        PrincipalSearcher ps = new PrincipalSearcher(findAllGroups);
                        foreach(var group in ps.FindAll()) {
                            if(userprinciple.IsMemberOf((GroupPrincipal)group)) {
                                userGroups.Add(group.SamAccountName);
                            }
                        }
                    }
                }
            } catch {}
            return userGroups;
        }

        public Permission GetUserInventoryPermission(UserPrincipal user)
        {
            try {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain,this.address,DomainManager.InventoryContainer, this.username, this.password)) {
                    GroupPrincipal findAllGroups = new GroupPrincipal(pc, "*");
                    PrincipalSearcher ps = new PrincipalSearcher(findAllGroups);
                    foreach (var group in ps.FindAll()) {
                        if (user.IsMemberOf((GroupPrincipal)group)) {
                            var permission = new Permission() {
                                Name=group.SamAccountName,
                                Description=group.Description
                            };
                            return permission;
                        }
                    }
                    return null;
                }
            } catch(ArgumentException e) {
                throw new LDAPCommunicationException("Internal Error: Error initializing LDAP Context, Please contact administrator.. " +
                            "See internal exception for more details", e);
            }catch(InvalidOperationException e) {
                throw new LDAPCommunicationException("Internal Error: Error in PrincipleSearcher, Please contact administrator.. " +
                            "See internal exception for more details",e);
            } catch {
                return null;
            }
        }

        public bool AddUserToGroup(string user, string groupName)
        {
            try {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, this.address, this.username, this.password)) {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, groupName);
                    group.Members.Add(pc, IdentityType.SamAccountName, user);
                    group.Save();
                    return true;
                }
            } catch {
                return false;
            }
        }

        public User GetDomainUser(string userName) {
            try {
                using(PrincipalContext pc = new PrincipalContext(ContextType.Domain, this.address, this.username, this.password)) {
                    UserPrincipal userprinciple = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName,userName);
                    var user = new User() {
                        FirstName = userprinciple.GivenName,
                        LastName = userprinciple.Surname,
                        Email = userprinciple.EmailAddress,
                        Extension = userprinciple.VoiceTelephoneNumber,
                        UserName = userprinciple.SamAccountName,
                        Permission = this.GetUserInventoryPermission(userprinciple)
                    };
                     return user;
                }
            } catch {
                return null;
            }
        }

        public IList<string> GetGroupMembers(string groupSamName)
        {
            try {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, this.address, this.username, this.password)) {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, groupSamName);
                    if (up != null) {
                        List<string> grpList = new List<string>();
                        up.GetGroups(pc).ToList().ForEach(grp => {
                            grpList.Add(grp.SamAccountName);
                        });
                        return grpList;
                    } else {
                        return null;
                    }
                }
            } catch {
                return null;
            }
        }

        public IList<User> GetSETUsers()
        {
            try {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain,this.address,DomainManager.SetiUserContainer,this.username, this.password)) {
                    UserPrincipal allUsers = new UserPrincipal(pc);
                    PrincipalSearcher ps = new PrincipalSearcher(allUsers);
                    List<User> users = new List<User>();
                    foreach(UserPrincipal user in ps.FindAll()) {
                        users.Add(new User() {
                            FirstName = user.GivenName,
                            LastName = user.Surname,
                            Email = user.EmailAddress,
                            Extension = user.VoiceTelephoneNumber,
                            UserName = user.SamAccountName
                        });
                    }
                    return users;
                }
            } catch {
                return null;
            }
        }

        public bool RemoveUserFromGroup(string user, string groupName)
        {
            try {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain,this.address, this.username, this.password)) {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, user);
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, groupName);
                    if (group.Members.Remove(pc, IdentityType.SamAccountName, user)) {
                        group.Save();
                        return true;
                    } else {
                        return false;
                    }
                }
            } catch {
                return false;
            }
        }

        public bool GetInventoryGroups()
        {
            try {
                using (PrincipalContext pc=new PrincipalContext(ContextType.Domain,this.address, DomainManager.InventoryContainer, this.username, this.password)) {
                    GroupPrincipal findAllGroups = new GroupPrincipal(pc, "*");
                    PrincipalSearcher ps = new PrincipalSearcher(findAllGroups);
                    Dictionary<string,string> groups = new Dictionary<string,string>();
                    foreach (var group in ps.FindAll()) {
                        groups.Add(group.SamAccountName, group.Description);
                    }
                    this._inventoryGroups = groups;
                    return true;
                }
            } catch {
                return false;
            }
        }

        public bool RemoveAllUserInventoryPermissions(string username) {
            var userPermissions = this.AllUserInventoryPermsions(username);
            foreach(var group in userPermissions) {
                if(!this.RemoveUserFromGroup(username, group)) {
                    return false;
                }
            }
            return true;
        }
    }
}
