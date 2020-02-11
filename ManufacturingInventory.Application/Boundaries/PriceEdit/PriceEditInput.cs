using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PriceEdit {
    public enum PriceEditAction {
        NEW,
        ReplaceWithNew,
        ReplaceWithExisiting,
        Edit,
        Delete
    }

    public class PriceEditInput {
        
        public PriceEditInput(int? priceId,int partInstanceId,int partId,PriceEditAction action) {
            this.PriceId = priceId;
            this.PartInstanceId = partInstanceId;
            this.PartId = partId;
            this.EditAction = action;
        }

        public PriceEditInput(DateTime timeStamp, DateTime? validFrom,
            DateTime? validUntil, bool isCurrent, double unitCost, 
            int minOrder, double leadTime, int distributorId, 
            int partId,PriceEditAction action, int? partInstanceId=null, int? priceId=null) {
            this.TimeStamp = timeStamp;
            this.ValidFrom = validFrom;
            this.ValidUntil = validUntil;
            this.IsCurrent = isCurrent;
            this.UnitCost = unitCost;
            this.MinOrder = minOrder;
            this.LeadTime = leadTime;
            this.PriceId = priceId;
            this.DistributorId = distributorId;
            this.PartInstanceId = partInstanceId;
            this.PartId = partId;
            this.EditAction = action;
        }

        public PriceEditAction EditAction { get; set; } 
        public int? PriceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsCurrent { get; set; }
        public double UnitCost { get; set; }
        public int MinOrder { get; set; }
        public double LeadTime { get; set; }

        public int DistributorId { get; set; }
        public int? PartInstanceId { get; set; }
        public int PartId { get; set; }
    }
}
