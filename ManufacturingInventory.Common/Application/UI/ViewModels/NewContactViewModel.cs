using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class NewContactViewModel {
               
        public NewContactViewModel() {
            this.Contact = new ContactDTO();
        }

        public virtual ContactDTO Contact { get; set; }
    }
}
