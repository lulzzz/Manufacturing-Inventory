using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Domain.Model;
using ManufacturingInventory.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ManufacturingInventory.Domain.Buisness.Concrete {
    public class UserService : IUserService {
        public User CurrentUser { get; set; }
        public Session CurrentSession { get; set; }
        public Permission UserPermission { get; set; }
        public List<UserAction> AvailableUserActions { get; private set; }

        public UserService(User currentUser, Session currentSession, Permission userPermission)
        {
            this.CurrentUser = currentUser;
            this.CurrentSession = currentSession;
            this.UserPermission = userPermission;
        }

        public UserService()
        {
            this.CurrentUser = null;
            this.CurrentSession = null;
            this.UserPermission = null;
        }

        public bool IsValid()
        {
            return (this.CurrentUser != null && this.CurrentSession != null);
        }

        private void Initialize() {
            this.AvailableUserActions = new List<UserAction>();
            if(this.UserPermission.Name == "InventoryAdminAccount") {
                this.AvailableUserActions.Add(UserAction.Add);
                this.AvailableUserActions.Add(UserAction.Edit);
                this.AvailableUserActions.Add(UserAction.Remove);
                this.AvailableUserActions.Add(UserAction.CheckIn);
                this.AvailableUserActions.Add(UserAction.CheckOut);
                this.AvailableUserActions.Add(UserAction.UserManagement);

            } else if(this.UserPermission.Name == "InventoryUserLimitedAccount") {
                this.AvailableUserActions.Add(UserAction.CheckIn);
                this.AvailableUserActions.Add(UserAction.CheckOut);

            } else if(this.UserPermission.Name == "InventoryUserFullAccount") {
                this.AvailableUserActions.Add(UserAction.Add);
                this.AvailableUserActions.Add(UserAction.Edit);
                this.AvailableUserActions.Add(UserAction.Remove);
                this.AvailableUserActions.Add(UserAction.CheckIn);
                this.AvailableUserActions.Add(UserAction.CheckOut);
            }
        }

        public bool Validate(UserAction action) {
            return this.AvailableUserActions.Contains(action);
        }

        public void LogOut()
        {
            using(var context=new ManufacturingContext()) {
                var entry=context.Entry<Session>(this.CurrentSession);
                entry.Entity.Out = DateTime.UtcNow;
                entry.State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
