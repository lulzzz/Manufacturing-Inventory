using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effortless.Net.Encryption;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Domain.Model;
using ManufacturingInventory.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.Domain.Buisness.Concrete {
    public class UserServiceProvider : IUserServiceProvider {

        public ManufacturingContext _context;
        public IDomainManager _domainManager;

        public UserServiceProvider(ManufacturingContext context, IDomainManager domainManager) {
            this._context = context;
            this._domainManager = domainManager;
            this.Load();
        }

        public void Load() {
            this._context.Users.Load();
            this._context.Users.Include(e => e.Permission).Load();
            this._context.Sessions.Load();
            this._context.Permissions.Load();
        }

        public IUserService CreateUserServiceUserAuthenticated(User user, bool storePassword, string permission = null, string password = null) {
            var userEntity = _context.Users.Include(e => e.Permission).FirstOrDefault(e => e.UserName == user.UserName);
            if (userEntity != null) {
                return this.CreateService(userEntity, storePassword, permission, password: password);
            } else {
                return this.CreateServiceNewUserEntity(user, storePassword, permission: permission, password: password);
            }
        }

        public IUserService CreateUserServiceNoPermissions(User user, bool storePassword, string password = null) {
            var userEntity = _context.Users.Include(e => e.Permission).FirstOrDefault(e => e.UserName == user.UserName);
            if (userEntity != null) {
                return this.CreateServiceDefualtPermissions(userEntity, storePassword, password);
            } else {
                return this.CreateServiceNewUserEntity(user, storePassword, password: password);
            }
        }

        private IUserService CreateService(User user, bool storePassword, string permission, string password = null) {
            if (user.Permission.Name == permission) {
                var userPermission = user.Permission;
                if (storePassword && !string.IsNullOrEmpty(password)) {
                    this.GeneratePasswordEncryption(password, user);
                }
                Session session = new Session(user);
                _context.Sessions.Add(session);
                _context.Entry<User>(user).State = EntityState.Modified;
                _context.SaveChanges();
                _context.Entry<User>(user).State = EntityState.Detached;
                _context.Entry<Permission>(userPermission).State = EntityState.Detached;
                _context.Entry<Session>(session).State = EntityState.Detached;
                return new UserService(user, session, userPermission);
            } else {
                var permissionEnity = _context.Permissions.First(e => e.Name == permission);
                if (permissionEnity != null) {
                    user.Permission = permissionEnity;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        this.GeneratePasswordEncryption(password, user);
                    }
                    Session session = new Session(user);
                    _context.Entry<User>(user).State = EntityState.Modified;
                    _context.Sessions.Add(session);
                    _context.SaveChanges();
                    _context.Entry<User>(user).State = EntityState.Detached;
                    _context.Entry<Permission>(permissionEnity).State = EntityState.Detached;
                    _context.Entry<Session>(session).State = EntityState.Detached;
                    return new UserService(user, session, permissionEnity);
                } else {
                    Permission newPermission = new Permission() {
                        Name = permission,
                        Description = ""
                    };
                    _context.Permissions.Add(newPermission);
                    user.Permission = newPermission;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        this.GeneratePasswordEncryption(password, user);
                    }
                    Session session = new Session(user);
                    _context.Sessions.Add(session);
                    _context.Entry<User>(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    _context.Entry<User>(user).State = EntityState.Detached;
                    _context.Entry<Permission>(newPermission).State = EntityState.Detached;
                    _context.Entry<Session>(session).State = EntityState.Detached;
                    return new UserService(user, session, newPermission);
                }
            }
        }

        private IUserService CreateServiceDefualtPermissions(User userEntity, bool storePassword, string password = null) {
            var permission = this._context.Permissions.FirstOrDefault(e => e.Name == LogInService.DefaultDomainPermissions);
            if (permission != null) {
                if (this._domainManager.AddUserToGroup(userEntity.UserName, LogInService.DefaultDomainPermissions)) {
                    userEntity.Permission = permission;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        this.GeneratePasswordEncryption(password, userEntity);
                    }
                    Session session = new Session(userEntity);
                    _context.Entry<User>(userEntity).State = EntityState.Modified;
                    _context.Sessions.Add(session);
                    _context.SaveChanges();
                    _context.Entry<User>(userEntity).State = EntityState.Detached;
                    _context.Entry<Permission>(permission).State = EntityState.Detached;
                    _context.Entry<Session>(session).State = EntityState.Detached;
                    return new UserService(userEntity, session, permission);
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        private IUserService CreateServiceNewUserEntity(User user, bool storePassword, string permission = null, string password = null) {
            if (!string.IsNullOrEmpty(permission)) {
                var permissionEnity = _context.Permissions.First(e => e.Name == permission);
                if (permissionEnity != null) {
                    user.Permission = permissionEnity;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        this.GeneratePasswordEncryption(password, user);
                    }
                    _context.Users.Add(user);
                    Session session = new Session(user);
                    _context.Sessions.Add(session);
                    _context.SaveChanges();
                    _context.Entry<User>(user).State = EntityState.Detached;
                    _context.Entry<Permission>(permissionEnity).State = EntityState.Detached;
                    _context.Entry<Session>(session).State = EntityState.Detached;
                    return new UserService(user, session, permissionEnity);
                } else {
                    Permission newPermission = new Permission() {
                        Name = permission,
                        Description = ""
                    };
                    _context.Permissions.Add(newPermission);
                    user.Permission = newPermission;
                    if (storePassword && !string.IsNullOrEmpty(password)) {
                        this.GeneratePasswordEncryption(password, user);
                    }
                    _context.Users.Add(user);
                    Session session = new Session(user);
                    _context.Sessions.Add(session);
                    _context.SaveChanges();
                    _context.Entry<User>(user).State = EntityState.Detached;
                    _context.Entry<Permission>(newPermission).State = EntityState.Detached;
                    _context.Entry<Session>(session).State = EntityState.Detached;
                    return new UserService(user, session, newPermission);
                }
            } else {
                _context.Users.Add(user);
                _context.SaveChanges();
                return this.CreateServiceDefualtPermissions(user, storePassword, password);
            }
        }

        private void GeneratePasswordEncryption(string password, User user) {
            //string decrypt/encrypt
            byte[] key = Bytes.GenerateKey();
            byte[] iv = Bytes.GenerateIV();
            string encrypted = Strings.Encrypt(password, key, iv);
            user.Key = key;
            user.IV = iv;
            user.EncryptedPassword = encrypted;
            user.StorePassword = true;
        }
    }
}
