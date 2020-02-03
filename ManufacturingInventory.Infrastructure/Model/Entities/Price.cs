using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class Price {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public double UnitCost { get; set; }
        public int MinOrder { get; set; }
        public double LeadTime { get; set; }
        public byte[] RowVersion { get; set; }

        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }

       //public int? AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        //public int? PartInstanceId { get; set; }
        //public PartInstance PartInstance { get; set; }

        //public ICollection<Attachment> Attachments { get; set; }
        public ICollection<PartInstance> PartInstances { get; set; }
        public ICollection<PartPrice> PartPrices { get; set; }
        public ICollection<PriceLog> PriceLogs { get; set; }

        public Price() {
            this.PartPrices = new HashSet<PartPrice>();
            this.PartInstances = new HashSet<PartInstance>();
            this.PriceLogs = new HashSet<PriceLog>();
        }

        public void Set(Price entity) {
            this.UnitCost = entity.UnitCost;
            this.DistributorId = entity.DistributorId;
            this.TimeStamp = entity.TimeStamp;
            this.MinOrder = entity.MinOrder;
            this.LeadTime = entity.LeadTime;
            this.ValidUntil = entity.ValidUntil;
            this.ValidFrom = entity.ValidFrom;
        }
    }
}
