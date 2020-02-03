using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PriceEdit {
    public class PriceEditInput {
        public PriceEditInput(DateTime timeStamp, DateTime? validFrom, DateTime? validUntil, 
            bool isCurrent, double unitCost, int minOrder, double leadTime, int distributorId, int partInstanceId) {
            this.TimeStamp = timeStamp;
            this.ValidFrom = validFrom;
            this.ValidUntil = validUntil;
            this.IsCurrent = isCurrent;
            this.UnitCost = unitCost;
            this.MinOrder = minOrder;
            this.LeadTime = leadTime;
            this.DistributorId = distributorId;
            this.PartInstanceId = partInstanceId;
        }

        public int PriceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsCurrent { get; set; }
        public double UnitCost { get; set; }
        public int MinOrder { get; set; }
        public double LeadTime { get; set; }

        public int DistributorId { get; set; }
        public int PartInstanceId { get; set; }
    }
}
