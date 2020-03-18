using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Domain.DTOs {
    public class ContactDTO : IContact {

        public ContactDTO() {

        }

        public ContactDTO(IContact contact) {
            this.Id = contact.Id;
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
        [Display(AutoGenerateField =false)]
        public int Id { get; set; }

        [Display(GroupName = "[Name]", Name = "First name")]
        public string FirstName { get; set; }

        [Display(GroupName = "[Name]", Name = "Last name")]
        public string LastName { get; set; }

        [Display(GroupName = "[Name]", Name = "Job Title", Order = 0)]
        public string Title { get; set; }

        [Display(GroupName = "{Tabs}/Contact", Name = "Country Code"), DataType(DataType.Text)]
        public string CountryCode { get; set; }
        [Display(GroupName = "{Tabs}/Contact", Name = "Phone"), DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Display(GroupName = "{Tabs}/Contact", Name = "Extension", Order = 1), DataType(DataType.Text)]
        public string Extension { get; set; }
        [Display(GroupName = "{Tabs}/Contact", Name = "Fax"), DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }
        [Display(GroupName = "{Tabs}/Contact", Name = "Email", Order = 2), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(GroupName = "{Tabs}/Contact/Address", Name = "Address", Order = 3), DataType(DataType.Text)]
        public string Address { get; set; }

        [Display(GroupName = "{Tabs}/Misc", Name = "Website", Order = 4), DataType(DataType.Text)]
        public string Website { get; set; }

        [Display(GroupName = "{Tabs}/Misc", Name = "Comments", Order = 5), DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}
