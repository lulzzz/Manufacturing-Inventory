using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    //public class Alert {
    //    public int Id { get; set; }
    //    public bool IsEnabled { get; set; }
    //    public DateTime AlertTime { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public StockType Stock { get; set; }
    //    public int PartId { get; set; }

    //    public ICollection<UserAlert> UserAlerts { get; set; }

    //    public Alert() {
    //        this.UserAlerts = new HashSet<UserAlert>();
    //    }
    //}

    public class Alert {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }

        public StockType Stock { get; set; }

        public ICollection<UserAlert> UserAlerts { get; set; }

        public Alert() {
            this.UserAlerts = new HashSet<UserAlert>();
        }
    }
}
