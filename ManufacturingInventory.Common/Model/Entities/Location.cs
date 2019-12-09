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
        public virtual ICollection<PartInstance> ItemsAtLocation { get; set; }

        public Location() {
            this.ItemsAtLocation = new HashSet<PartInstance>();
        }
    }

    public partial class Warehouse : Location {
        public ICollection<Part> StoredParts { get; set; }
        public ICollection<IncomingTransaction> IncomingTransactions { get; set; }

        public Warehouse() {
            this.StoredParts = new ObservableHashSet<Part>();
            this.IncomingTransactions = new ObservableHashSet<IncomingTransaction>();
        }
    }

    public partial class Consumer : Location {
        public ICollection<OutgoingTransaction> OutgoingTransactions { get; set; }

        public Consumer() {
            this.OutgoingTransactions = new ObservableHashSet<OutgoingTransaction>();
        }
    }
}
