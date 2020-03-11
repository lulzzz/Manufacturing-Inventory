using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ManufacturingInventory.Domain.DTOs {
    public class ContactDTO {

        [Display(GroupName = "[Name]", Name = "First name")]
        public string FirstName { get; set; }

        [Display(GroupName = "[Name]", Name = "Last name")]
        public string LastName { get; set; }

        [Display(GroupName = "[Name]", Name = "Job Title", Order = 0)]
        public string Title { get; set; }

        [Display(GroupName = "{Tabs}/Contact",Name="Country Code"),DataType(DataType.PhoneNumber)]
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

        [Display(GroupName = "{Tabs}/Misc", Name = "Website", Order = 4), DataType(DataType.Url)]
        public string Website { get; set; }

        [Display(GroupName = "{Tabs}/Misc", Name = "Comments", Order =5), DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}
