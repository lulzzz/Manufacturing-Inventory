using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using System.ComponentModel;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public enum CategoryTypes {
        [Description("Organization")] Organization,
        [Description("Condition")] Condition,
        [Description("StockType")] StockType,
        [Description("Usage")] Usage,
        [Description("InvalidType")]InvalidType
    }

    public abstract class Category:ICategory {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public byte[] RowVersion { get; set; }

        public Category() {

        }

        public Category(string name,string description) {
            this.Name = name;
            this.Description = description;
        }

        public Category(ICategory category) {
            this.Name = category.Name;
            this.Description = category.Description;
            this.IsDefault = category.IsDefault;
        }

        public void Set(Category category) {
            this.Name = category.Name;
            this.Description = category.Description;
            this.IsDefault = category.IsDefault;
        }

        public virtual void Set(ICategory category) {
            this.Name = category.Name;
            this.Description = category.Description;
            this.IsDefault = category.IsDefault;
        }
    }

    public partial class Organization : Category {
        public virtual ICollection<Part> Parts { get; set; }

        public Organization() {
            this.Parts = new HashSet<Part>();
        }

        public Organization(string name, string description) :base(name,description) {
            this.Parts = new HashSet<Part>();
        }

        public Organization(ICategory organization) : base(organization) { }
    }

    public partial class Condition : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public Condition() {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public Condition(string name, string description) : base(name, description) {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public Condition(ICategory condition):base(condition) {
            this.PartInstances = new HashSet<PartInstance>();
        }
    }

    public partial class Usage : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public Usage() {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public Usage(string name, string description) : base(name, description) {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public Usage(ICategory usage):base(usage) {
            this.PartInstances = new HashSet<PartInstance>();
        }
    }

    public partial class StockType : Category {
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public bool HoldsBubblers { get; set; }

        public int? CombinedAlertId { get; set; }
        public CombinedAlert CombinedAlert { get; set; }

        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public StockType() {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public StockType(string name, string description) : base(name, description) {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public StockType(ICategory stockType):base(stockType) {
            this.PartInstances = new HashSet<PartInstance>();
            Type catType = stockType.GetType();
            if (catType.Equals(typeof(StockType))) {
                this.MinQuantity = ((StockType)stockType).MinQuantity;
                this.SafeQuantity = ((StockType)stockType).SafeQuantity;
                this.Quantity = ((StockType)stockType).Quantity;
                this.HoldsBubblers= ((StockType)stockType).HoldsBubblers;
            }
        }

        //public override void Set(ICategory category) {
        //    Type catType = category.GetType();
        //    if (catType.Equals(typeof(StockType))) {
        //        this.MinQuantity = ((StockType)category).MinQuantity;
        //        this.SafeQuantity = ((StockType)category).SafeQuantity;
        //        this.Quantity = ((StockType)category).Quantity;
        //    }
        //}
    }
}
