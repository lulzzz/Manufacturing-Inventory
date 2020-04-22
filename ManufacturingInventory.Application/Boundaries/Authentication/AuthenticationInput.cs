using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Authentication {
    public class AuthenticationInput {

        public AuthenticationInput(string userName,string password, AuthenticationActions action,bool saveCredentials=false) {
            this.UserName = userName;
            this.Password = password;
            this.Action = action;
            this.SaveCredentials = saveCredentials;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SaveCredentials { get; set; }
        public AuthenticationActions Action { get; set; }
    }
}
