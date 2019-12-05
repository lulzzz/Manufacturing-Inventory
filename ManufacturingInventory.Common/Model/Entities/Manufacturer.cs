using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public class Manufacturer {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Part> Parts { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }

        public Manufacturer() {
            this.Contacts = new ObservableHashSet<Contact>();
            this.Attachments = new ObservableHashSet<Attachment>();
        }

        public Manufacturer(string name, string description, string comments) : this() {
            this.Name = name;
            this.Description = description;
            this.Comments = comments;
        }
    }
}
