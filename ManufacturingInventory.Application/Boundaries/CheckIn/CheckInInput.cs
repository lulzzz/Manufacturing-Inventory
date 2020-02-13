using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public class CheckInInput {

        public CheckInInput(PartInstance partInstance, bool createTransaction) {
            this.PartInstance = partInstance;
            this.CreateTransaction = createTransaction;
        }

        public PartInstance PartInstance { get; set; }
        public bool CreateTransaction { get; set; }

    }
}
