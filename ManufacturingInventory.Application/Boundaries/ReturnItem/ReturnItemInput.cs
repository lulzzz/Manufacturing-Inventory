using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.ReturnItem {
    public class ReturnItemInput {
        public ReturnItemInput(DateTime timeStamp, int quantity, bool isBubbler, int partInstanceId, 
            int locationId, int referenceTransactionId, double weight=0, double measuredWeight=0, int conditionId=0) {
            this.TimeStamp = timeStamp;
            this.Quantity = quantity;
            this.IsBubbler = isBubbler;
            this.PartInstanceId = partInstanceId;
            this.LocationId = locationId;
            this.ConditionId = conditionId;
            this.ReferenceTransactionId = referenceTransactionId;
            this.Weight = weight;
            this.MeasuredWeight = measuredWeight;
        }

        public DateTime TimeStamp { get; set; }
        public int Quantity { get; set; }
        public bool IsBubbler { get; set; }
        public int PartInstanceId { get; set; }
        public int LocationId { get; set; }
        public int ConditionId { get; set; }
        public int ReferenceTransactionId { get; set; }
        public double Weight { get; set; }
        public double MeasuredWeight { get; set; }
    }

}
