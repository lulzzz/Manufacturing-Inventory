using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public class User {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Extension { get; set; }
        public bool StorePassword { get; set; }
        public string EncryptedPassword { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] RowVersion { get; set; }

        public int? PermissionId { get; set; }
        public Permission Permission { get; set; }

        public ICollection<UserAlert> UserAlerts { get; set; }

        public ICollection<Session> Sessions { get; set; }

        public User() {
            this.UserAlerts = new ObservableHashSet<UserAlert>();
            this.Sessions = new ObservableHashSet<Session>();
        }
    }
}
