using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public abstract class Category {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public Category() {

        }

        public Category(string name,string description) {
            this.Name = name;
            this.Description = description;
        }

        public Category(Category category) {
            this.Name = category.Name;
            this.Description = category.Description;
        }

        public void Set(Category category) {
            this.Name = category.Name;
            this.Description = category.Description;
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

        public Organization(Organization organization) : base(organization) { }
    }

    public partial class Condition : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public Condition() {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public Condition(string name, string description) : base(name, description) {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public Condition(Condition condition):base(condition) {
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

        public Usage(Usage usage):base(usage) {
            this.PartInstances = new HashSet<PartInstance>();
        }
    }

    public partial class StockType : Category {
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public StockType() {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public StockType(string name, string description) : base(name, description) {
            this.PartInstances = new HashSet<PartInstance>();
        }

        public StockType(StockType type):base(type) {
            this.PartInstances = new HashSet<PartInstance>();
            this.MinQuantity = type.MinQuantity;
            this.SafeQuantity = type.SafeQuantity;
            this.Quantity = type.Quantity;
        }

        public void UpdateQuantity() {
            if (this.PartInstances != null) {
                this.Quantity = this.PartInstances.ToList().Sum(instance => {
                    if (instance.IsBubbler) {
                        return instance.Quantity;
                    } else {
                        return Convert.ToInt32(instance.BubblerParameter.NetWeight);
                    }
                });
            }
        }
    }

    //public partial class Designation : Category {
    //    public virtual ICollection<PartInstance> PartInstances { get; set; }

    //    public Designation() {
    //        this.PartInstances = new HashSet<PartInstance>();
    //    }

    //    public Designation(string name,string description) {
    //        this.Name = name;
    //        this.Description = description;
    //    }
    //}
}
