using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public class PartInstance {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SkuNumber { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }
        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public bool CostReported { get; set; }
        public bool IsResuable { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int? PartTypeId { get; set; }
        public PartType PartType { get; set; }

        public int? ConditionId { get; set; }
        public Condition Condition { get; set; }

        public int LocationId { get; set; }
        public Location CurrentLocation { get; set; }

        public InstanceParameter InstanceParameter { get; set; }

        public Price Price { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public PartInstance() {
            this.Transactions = new ObservableHashSet<Transaction>();
            this.Attachments = new ObservableHashSet<Attachment>();
        }

        public PartInstance(Part part, string name, string serialNumber, string batchNumber, string skuNumber) : this() {
            this.Name = name;
            this.SerialNumber = serialNumber;
            this.BatchNumber = batchNumber;
            this.SkuNumber = skuNumber;
            this.Part = part;
        }
    }
}
