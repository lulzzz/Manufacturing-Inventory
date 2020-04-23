using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Authentication {
    public class AuthenticationInput {

        public AuthenticationInput(string userName, string password, AuthenticationActions action, bool saveCredentials = false) {
            this.UserName = userName;
            this.Password = password;
            this.Action = action;
            this.SaveCredentials = saveCredentials;
            this.UserService = null;
        }

        public AuthenticationInput(UserService userService, AuthenticationActions action) {
            this.UserName = String.Empty;
            this.Password = String.Empty;
            this.Action = action;
            this.SaveCredentials = false;
            this.UserService = userService;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SaveCredentials { get; set; }
        public IUserService UserService { get; set; }
        public AuthenticationActions Action { get; set; }
    }
}
