using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.ContactTableDetailEdit {
    public class ContactTableDetailEditInput {
        public ContactTableDetailEditInput() {

        }

        public ContactTableDetailEditInput(EditAction editAction, ContactDTO contact, int distributorId) {
            this.EditAction = editAction;
            this.Contact = contact;
            this.DistributorId = distributorId;
        }

        public EditAction EditAction { get; set; }
        public ContactDTO Contact { get; set; }
        public int DistributorId { get; set; }
    }
}
