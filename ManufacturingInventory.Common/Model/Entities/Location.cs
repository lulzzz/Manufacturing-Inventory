using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public abstract class Location {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }
        //public virtual ICollection<Transaction> Transactions { get; set; }
        //public virtual ICollection<PartInstance> ItemsAtLocation { get; set; }

        public Location() {
            //this.ItemsAtLocation = new HashSet<PartInstance>();
            //this.Transactions = new HashSet<Transaction>();
        }
    }

    public partial class Warehouse : Location {
        public ICollection<Part> StoredParts { get; set; }

        public Warehouse() {
            //this.Transactions = new HashSet<Transaction>();
            //this.StoredItems = new HashSet<Part>();
        }
    }

    public partial class Consumer : Location {

        public Consumer() {
            //this.ItemsAtLocation = new HashSet<PartInstance>();
            //this.Transactions = new HashSet<Transaction>();
        }
    }
}
