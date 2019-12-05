using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public class Part {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? OgranizationId { get; set; }
        public virtual Organization Organization { get; set; }

        public int? WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public int? UsageId { get; set; }
        public Usage Usage { get; set; }

        public ICollection<Manufacturer> Manufacturers { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<PartInstance> PartInstances { get; set; }
        public ICollection<PartManufacturer> PartManufacturers { get; set; }

        public Part() {
            this.Manufacturers = new ObservableHashSet<Manufacturer>();
            this.Attachments = new ObservableHashSet<Attachment>();
            this.PartInstances = new ObservableHashSet<PartInstance>();
            this.PartManufacturers = new ObservableHashSet<PartManufacturer>();
        }
    }
}
