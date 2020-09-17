using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class TransactionDTO : ITransaction {
        public TransactionDTO(DateTime timeStamp, InventoryAction inventoryAction,
            int quantity, bool isReturning, double unitCost, double totalCost,
            int partInstanceId, string partInstanceName, string locationName,
            int locationId, bool isBubbler = false, double measured = 0, double weight = 0,
            int referenceTransactionId = 0, int conditionId = 0) {

            this.TimeStamp = timeStamp;
            this.InventoryAction = inventoryAction;
            this.Quantity = quantity;
            this.Measured = measured;
            this.Weight = weight;
            this.IsReturning = isReturning;
            this.UnitCost = unitCost;
            this.TotalCost = totalCost;
            this.PartInstanceId = partInstanceId;
            this.PartInstanceName = partInstanceName;
            this.LocationName = locationName;
            this.LocationId = locationId;
            this.IsBubbler = isBubbler;

            if (referenceTransactionId != 0)
                this.ReferenceTransactionId = referenceTransactionId;

            this.ConditionId = conditionId;
        }

        public TransactionDTO() {

        }

        public TransactionDTO(Transaction transaction) {
            this.TimeStamp = transaction.TimeStamp;
            this.InventoryAction = transaction.InventoryAction;
            this.Quantity = (transaction.PartInstance.IsBubbler) ? (int)transaction.Weight : transaction.Quantity;
            this.Measured = transaction.Weight;
            this.UnitCost = transaction.UnitCost;
            this.TotalCost = transaction.TotalCost;
            this.PartInstanceId = transaction.PartInstanceId;
            this.PartInstanceName =(transaction.PartInstance!=null) ? transaction.PartInstance.Name:"Instance Not Loaded";
            this.LocationId = transaction.LocationId;
            this.LocationName =(transaction.Location!=null) ? transaction.Location.Name:"Location Not Loaded";
            //this.IsBubbler needs check for PartInstance!=null.  Not needed for this use case
        }

        public DateTime TimeStamp { get; set; }
        public InventoryAction InventoryAction { get; set; }
        public int Quantity { get; set; }
        public double Measured { get; set; }
        public double Weight { get; set; }
        public bool IsReturning { get; set; }
        public bool IsBubbler { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }
        public int PartInstanceId { get; set; }
        public string PartInstanceName { get; set; }
        public string LocationName { get; set; }
        public int LocationId { get; set; }
        public int ReferenceTransactionId { get; set; }
        public int ConditionId { get; set; }

    }
}
