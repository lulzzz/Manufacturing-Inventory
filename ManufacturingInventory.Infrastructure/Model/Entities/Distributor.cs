﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    //public class Distributor {
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public virtual ICollection<Price> Prices { get; set; }
    //    public virtual ICollection<Contact> Contacts { get; set; }
    //    public virtual ICollection<Attachment> Attachments { get; set; }

    //    //Tracking
    //    public DateTime? Inserted { get; set; }
    //    public DateTime? Updated { get; set; }

    //    public Distributor() {
    //        this.Prices = new ObservableHashSet<Price>();
    //        this.Contacts = new ObservableHashSet<Contact>();
    //        this.Attachments = new ObservableHashSet<Attachment>();
    //    }

    //    public Distributor(string name,string description) {
    //        this.Name = name;
    //        this.Description = description;
    //    }
    //}

    public class Distributor {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }

        //public Price Price { get; set; }

        public ICollection<Price> Prices { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Attachment> Attachments { get; set; }

        public Distributor() {
            this.Prices = new ObservableHashSet<Price>();
            this.Contacts = new ObservableHashSet<Contact>();
            this.Attachments = new ObservableHashSet<Attachment>();
        }

        public Distributor(string name, string description):this() {
            this.Name = name;
            this.Description = description;
        }
    }
}