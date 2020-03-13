using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.Services {
    public class ContactDataTraveler {

        public ContactDataTraveler() {
        }

        public ContactDataTraveler(int distributorId, bool canEdit) {
            this.DistributorId = distributorId;
            this.CanEdit = canEdit;
        }

        public int DistributorId { get; set; }
        public bool CanEdit { get; set; }
    }
}
