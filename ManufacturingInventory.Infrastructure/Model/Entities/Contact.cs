using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class Contact {
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
    }
}
