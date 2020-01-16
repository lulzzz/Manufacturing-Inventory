using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class Manufacturer {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<PartManufacturer> PartManufacturers { get; set; }

        public Manufacturer() {
            this.Contacts = new ObservableHashSet<Contact>();
            this.Attachments = new ObservableHashSet<Attachment>();
            this.PartManufacturers = new ObservableHashSet<PartManufacturer>();
        }

        public Manufacturer(string name, string description, string comments) : this() {
            this.Name = name;
            this.Description = description;
            this.Comments = comments;
        }
    }
}
