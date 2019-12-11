using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public abstract class Category {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        public Category() {

        }
    }

    public partial class Organization : Category {
        public virtual ICollection<Part> Parts { get; set; }

        public Organization() {
            this.Parts = new ObservableHashSet<Part>();
        }

        public Organization(string name, string description) : this() {
            this.Name = name;
            this.Description = description;
        }
    }

    public partial class Condition : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }

        public Condition() {
            this.PartInstances = new ObservableHashSet<PartInstance>();
        }
    }

    public partial class Usage : Category {
        public virtual ICollection<Part> Parts { get; set; }

        public Usage() {
            this.Parts = new ObservableHashSet<Part>();
        }
    }

    public partial class PartType : Category {
        public virtual ICollection<PartInstance> PartInstances { get; set; }
        public PartType() {
            this.PartInstances = new ObservableHashSet<PartInstance>();
        }
    }
}
