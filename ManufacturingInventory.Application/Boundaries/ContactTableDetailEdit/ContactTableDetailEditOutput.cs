using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.ContactTableDetailEdit {
    public class ContactTableDetailEditOutput : IOutput {

        public ContactTableDetailEditOutput() {

        }

        public ContactTableDetailEditOutput(Contact contact,bool success, string message) {
            this.Success = success;
            this.Message = message;
            this.Contact = contact;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public Contact Contact { get; set; }
    }
}
