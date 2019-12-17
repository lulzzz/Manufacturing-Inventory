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

    public abstract class Transaction {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public InventoryAction InventoryAction { get; set; }
        public bool IsReturning { get; set; }
        public byte[] RowVersion { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public int Quantity { get; set; }

        public double InstanceParameterValue { get; set; }

    }

    public class OutgoingTransaction:Transaction {
        public int? ConsumerId { get; set; }
        public Consumer Consumer { get; set; }

        public int? ReturningTransactionId { get; set; }
        public ReturningTransaction ReturningTransaction { get; set; }
    }

    public class IncomingTransaction: Transaction {
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

    }

    public class ReturningTransaction: Transaction {
        public int? OutgoingTransactionId { get; set; }
        public OutgoingTransaction OutgoingTransaction { get; set; }
        
    }
}
