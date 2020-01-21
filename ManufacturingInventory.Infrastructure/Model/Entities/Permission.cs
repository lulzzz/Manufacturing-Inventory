using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class Permission {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<User> Users { get; set; }

        public Permission() {
            this.Users = new HashSet<User>();
        }

        public Permission(string name,string description) : this() {
            this.Name = name;
            this.Description = description;
        }
    }
}
