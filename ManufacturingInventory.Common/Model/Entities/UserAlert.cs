using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public class UserAlert {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int AlertId { get; set; }
        public Alert Alert { get; set; }
    }
}
