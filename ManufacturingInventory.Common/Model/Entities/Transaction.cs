using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {

    public enum InventoryAction : int {
        OUTGOING,
        INCOMING,
        RETURNING
    }

    //public class Transaction {
    //    public int Id { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //    public InventoryAction InventoryAction { get; set; }
    //    public bool IsReturning { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public int? OutgoingTransactionId { get; set; }
    //    public virtual Transaction OutgoingTransaction { get; set; }

    //    public int SessionId { get; set; }
    //    public virtual Session Session { get; set; }

    //    public int PartInstanceId { get; set; }
    //    public virtual PartInstance PartInstance { get; set; }

    //    public int LocationId { get; set; }
    //    public virtual Location Location { get; set; }

    //    public int Quantity { get; set; }

    //    public double TrackedValue { get; set; }

    //    public int? InstanceParameterId { get; set; }
    //    public virtual InstanceParameter InstanceParameter { get; set; }

    //}

    public class Transaction:ICloneable {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public InventoryAction InventoryAction { get; set; }
        public int Quantity { get; set; }
        public double ParameterValue { get; set; }
        public bool IsReturning { get; set; }
        public byte[] RowVersion { get; set; }

        public double UnitCost { get; set; }
        public double TotalCost { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public int? LocationId { get; set; }
        public Location Location { get; set; }

        public int? ReferenceTransactionId { get; set; }
        public Transaction ReferenceTransaction { get; set; }

        [NotMapped]
        public double Consumed {
            get {
                if (this.ReferenceTransaction != null) {
                    return this.ReferenceTransaction.ParameterValue - this.ParameterValue;
                } else {
                    return 0;
                }
            }
        }


        public Transaction() {
            this.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Inventory Transaction with Tracked Value
        /// </summary>
        /// <param name="instance">Part Instance</param>
        /// <param name="inventoryAction">Outgoing,Incoming,Returning</param>
        /// <param name="parameterValue">Tracked Value</param>
        /// <param name="isReturning">Is Item Returning</param>
        /// <param name="location">Where Item is Going</param>
        public Transaction(PartInstance instance, InventoryAction inventoryAction, double parameterValue, bool isReturning, Location location) {
            this.TimeStamp = DateTime.Now;
            this.PartInstance = instance;
            this.ParameterValue = parameterValue;
            this.UnitCost = this.PartInstance.UnitCost;
            this.TotalCost = (this.PartInstance.IsBubbler) ? (this.PartInstance.TotalCost) : (this.Quantity * this.UnitCost);
            this.InventoryAction = inventoryAction;
            this.IsReturning = isReturning;
            this.Location = location;
            this.PartInstance.CurrentLocation = location;
            this.Quantity = 1;
            this.PartInstance.CostReported = false;
            this.PartInstance.Quantity = 0;

        }

        public Transaction(PartInstance instance, InventoryAction inventoryAction,DateTime timeStamp, double parameterValue, bool isReturning, Location location) {
            this.PartInstanceId = instance.Id;
            this.PartInstance = instance;
            this.ParameterValue = parameterValue;
            this.UnitCost = instance.UnitCost;
            this.TotalCost = (instance.IsBubbler) ? (instance.TotalCost) : (this.Quantity * this.UnitCost);
            this.TimeStamp = timeStamp;
            this.InventoryAction = inventoryAction;
            this.IsReturning = isReturning;
            this.LocationId = location.Id;
            this.Quantity = 1;
        }

        /// <summary>
        /// Returning Inventory Transaction
        /// </summary>
        /// <param name="instance">Part Instance</param>
        /// <param name="inventoryAction"></param>
        /// <param name="parameterValue"></param>
        /// <param name="location"></param>
        /// <param name="referenceTransaction"></param>
        public Transaction(PartInstance instance,InventoryAction inventoryAction, double parameterValue, Location location, Transaction referenceTransaction) {
            this.PartInstance = instance;
            this.ParameterValue = parameterValue;
            this.UnitCost = this.PartInstance.UnitCost;
            this.TotalCost = (this.PartInstance.IsBubbler) ? (this.ParameterValue * this.UnitCost) : (this.Quantity*this.UnitCost);
            this.TimeStamp = DateTime.Now;
            this.InventoryAction = inventoryAction;
            this.ParameterValue = parameterValue;
            this.Location = location;
            this.ReferenceTransaction = referenceTransaction;
        }

        /// <summary>
        /// Standard Inventory Transaction, Quantity
        /// </summary>
        /// <param name="instance">Part Instance</param>
        /// <param name="inventoryAction">Outgoing,Incoming,Returning</param>
        /// <param name="quantity">Quantity of Item</param>
        /// <param name="location">Where Item is Going</param>
        public Transaction(PartInstance instance,InventoryAction inventoryAction, int quantity, Location location) {
            this.TimeStamp = DateTime.Now;
            this.InventoryAction = inventoryAction;
            this.Quantity = quantity;
            this.UnitCost = this.PartInstance.UnitCost;
            this.TotalCost = this.Quantity * this.UnitCost;
            this.Location = location;
            this.Quantity = 1;
            this.IsReturning = false;
        }

        public Transaction(PartInstance instance, InventoryAction inventoryAction) {
            this.PartInstance = instance;
            this.PartInstanceId = instance.Id;
            this.UnitCost = instance.UnitCost;
            this.TimeStamp = DateTime.Now;
            if (instance.IsBubbler) {
                this.Quantity = 1;
            }

            if (instance.CostReported) {
                this.UnitCost = instance.UnitCost;
                this.TotalCost = instance.TotalCost;
            }

            this.InventoryAction = inventoryAction;
        }

        public void Set(Transaction transaction) {
            this.TimeStamp = transaction.TimeStamp;
            this.InventoryAction = transaction.InventoryAction;
            this.Quantity = transaction.Quantity;
            this.ParameterValue = transaction.ParameterValue;
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
