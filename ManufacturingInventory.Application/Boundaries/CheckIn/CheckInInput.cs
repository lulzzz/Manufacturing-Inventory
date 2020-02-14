using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public class CheckInInput {

        public CheckInInput(PartInstance partInstance,DateTime timeStamp,bool createTransaction) {
            this.PartInstance = partInstance;
            this.CreateTransaction = createTransaction;
            this.TimeStamp = timeStamp;
        }

        public PartInstance PartInstance { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool CreateTransaction { get; set; }

    }
}
