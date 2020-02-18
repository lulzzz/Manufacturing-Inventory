using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public class CheckInInput {

        public CheckInInput(PartInstance partInstance,bool createTransaction,bool createNewPrice,DateTime? transactionTimeStamp=null) {
            this.PartInstance = partInstance;
            this.CreateTransaction = createTransaction;
            this.TimeStamp = transactionTimeStamp;
            this.CreateNewPrice = createNewPrice;
        }

        public PartInstance PartInstance { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool CreateTransaction { get; set; }
        public bool CreateNewPrice { get; set; }

    }
}
