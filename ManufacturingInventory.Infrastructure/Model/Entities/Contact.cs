using ManufacturingInventory.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class Contact:IContact {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Extension { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public byte[] RowVersion { get; set; }

        public int? ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }

        public int? DistributorId { get; set; }
        public virtual Distributor Distributor { get; set; }
       
        public Contact() {

        }

        public Contact(IContact contact) {
            this.FirstName = contact.FirstName;
            this.LastName = contact.LastName;
            this.Address = contact.Address;
            this.Phone = contact.Phone;
            this.Fax = contact.Fax;
            this.Website = contact.Website;
            this.Extension = contact.Extension;
            this.Comments = contact.Extension;
            this.Title = contact.Title;
            this.Email = contact.Email;
            this.CountryCode = contact.CountryCode;
        }

        public void Set(IContact contact) {
            this.FirstName = contact.FirstName;
            this.LastName = contact.LastName;
            this.Address = contact.Address;
            this.Phone = contact.Phone;
            this.Fax = contact.Fax;
            this.Website = contact.Website;
            this.Extension = contact.Extension;
            this.Comments = contact.Extension;
            this.Title = contact.Title;
            this.Email = contact.Email;
            this.CountryCode = contact.CountryCode;
        }
    }
}
