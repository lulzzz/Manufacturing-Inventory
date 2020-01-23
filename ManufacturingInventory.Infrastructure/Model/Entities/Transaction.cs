using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {

    public enum InventoryAction : int {
        OUTGOING,
        INCOMING,
        RETURNING
    }

    public class Transaction:ICloneable {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public InventoryAction InventoryAction { get; set; }
        public int Quantity { get; set; }
        public double MeasuredWeight { get; set; }
        public double Weight { get; set; }
        public byte[] RowVersion { get; set; }

        public double UnitCost { get; set; }
        public double TotalCost { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public int? ReferenceTransactionId { get; set; }
        public Transaction ReferenceTransaction { get; set; }

        [NotMapped]
        public double Consumed {
            get {
                if (this.ReferenceTransaction != null) {
                    return this.ReferenceTransaction.Weight - this.Weight;
                } else {
                    return 0;
                }
            }
        }


        public Transaction() {
            this.TimeStamp = DateTime.Now;
        }


        public Transaction(PartInstance instance, InventoryAction inventoryAction,double measured, double weight, Location location,DateTime timeStamp) {
            this.TimeStamp = timeStamp;
            this.Weight = weight;
            this.MeasuredWeight = measured;
            this.UnitCost = instance.UnitCost;
            this.TotalCost = (instance.IsBubbler) ? (instance.TotalCost) : (this.Quantity * this.UnitCost);
            this.InventoryAction = inventoryAction;
            this.Quantity = 1;
            this.LocationId = location.Id;
            this.PartInstanceId = instance.Id;
        }


        public void Set(Transaction transaction) {
            this.TimeStamp = transaction.TimeStamp;
            this.InventoryAction = transaction.InventoryAction;
            this.Quantity = transaction.Quantity;
            this.Weight = transaction.Weight;
            this.UnitCost = transaction.UnitCost;
            this.TotalCost = transaction.UnitCost;
            this.SessionId = transaction.SessionId;
            this.PartInstanceId = transaction.PartInstanceId;
            this.LocationId = transaction.LocationId;
            this.ReferenceTransactionId = transaction.ReferenceTransactionId;
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
