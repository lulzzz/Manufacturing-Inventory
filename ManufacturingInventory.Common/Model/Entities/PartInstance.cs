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

        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public bool IsResuable { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        public int? PartTypeId { get; set; }
        public virtual PartType PartType { get; set; }

        public int? ConditionId { get; set; }
        public virtual Condition Condition { get; set; }

        public virtual ICollection<Price> Prices { get; set; }
        public virtual ICollection<InstanceParameter> InstanceParameters { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

        public PartInstance() {
            this.InstanceParameters = new HashSet<InstanceParameter>();
            this.Transactions = new HashSet<Transaction>();
            this.Prices = new HashSet<Price>();
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
