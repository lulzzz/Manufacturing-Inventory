using ManufacturingInventory.Infrastructure.Model.Entities;
using System;

namespace ManufacturingInventory.Infrastructure.Model.Interfaces {
    public interface ITransaction {
        int ConditionId { get; set; }
        InventoryAction InventoryAction { get; set; }
        bool IsBubbler { get; set; }
        bool IsReturning { get; set; }
        int LocationId { get; set; }
        string LocationName { get; set; }
        double Measured { get; set; }
        int PartInstanceId { get; set; }
        string PartInstanceName { get; set; }
        int Quantity { get; set; }
        int ReferenceTransactionId { get; set; }
        DateTime TimeStamp { get; set; }
        double TotalCost { get; set; }
        double UnitCost { get; set; }
        double Weight { get; set; }
    }
}