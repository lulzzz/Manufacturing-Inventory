using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class Alert {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime AlertTime { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public ICollection<UserAlert> UserAlerts { get; set; }

        public Alert() {
            this.UserAlerts = new HashSet<UserAlert>();
        }
    }
}
