using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class PriceLog {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsCurrent { get; set; }
        public byte[] RowVersion { get; set; }


        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public int PriceId { get; set; }
        public Price Price { get; set; }

    }
}
