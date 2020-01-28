using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.ReturnItem {
    public class ReturnItemInput {
        public ReturnItemInput(DateTime timeStamp, int quantity, int partInstanceId, int locationId, int conditionId, int referenceTransactionId) {
            this.TimeStamp = timeStamp;
            this.Quantity = quantity;
            this.PartInstanceId = partInstanceId;
            this.LocationId = locationId;
            this.ConditionId = conditionId;
            this.ReferenceTransactionId = referenceTransactionId;
        }

        public DateTime TimeStamp { get; set; }
        public int Quantity { get; set; }

        public int PartInstanceId { get; set; }
        public int LocationId { get; set; }
        public int ConditionId { get; set; }
        public int ReferenceTransactionId { get; set; }
    }

    public class ReturnBubblerInput:ReturnItemInput {
        public ReturnBubblerInput(DateTime timeStamp, int quantity, int partInstanceId, 
            int locationId, int conditionId, int referenceTransactionId, double measured,double weight)
            :base(timeStamp, quantity, partInstanceId, locationId, conditionId, referenceTransactionId) {
            this.Weight = weight;
            this.MeasuredWeight = measured;
        }

        public double Weight { get; set; }
        public double MeasuredWeight { get; set; }
    }


}
