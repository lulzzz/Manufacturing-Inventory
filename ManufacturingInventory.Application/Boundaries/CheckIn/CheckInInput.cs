using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public class CheckInInput {

        public CheckInInput(PartInstance partInstance,bool createNewPrice,DateTime transactionTimeStamp) {
            this.PartInstance = partInstance;
            this.TimeStamp = transactionTimeStamp;
            this.CreateNewPrice = createNewPrice;
        }

        public CheckInInput(PartInstance partInstance, bool createNewPrice, DateTime timeStamp, Price price=null,BubblerParameter bubblerParameter = null) {
            this.PartInstance = partInstance;
            this.TimeStamp = timeStamp;
            this.CreateNewPrice = createNewPrice;
            this.Price = price;
            this.BubblerParameter = bubblerParameter;
        }

        public PartInstance PartInstance { get; set; }
        public Price Price { get; set; }
        public  BubblerParameter BubblerParameter { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool CreateNewPrice { get; set; }

    }
}
