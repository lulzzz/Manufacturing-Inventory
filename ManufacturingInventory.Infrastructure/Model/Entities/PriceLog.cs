using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class PriceLog {

        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsCurrent { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public int PriceId { get; set; }
        public Price Price { get; set; }

        public PriceLog() {

        }

        public PriceLog(DateTime timeStamp, bool isCurrent, PartInstance partInstance, Price price) {
            this.TimeStamp = timeStamp;
            this.IsCurrent = isCurrent;
            this.PartInstance = partInstance;
            this.Price = price;
        }

        public PriceLog(PartInstance partInstance, Price price) {
            this.TimeStamp = price.TimeStamp;
            this.IsCurrent = true;
            this.PartInstance = partInstance;
            this.Price = price;
        }

        public PriceLog(DateTime timeStamp, bool isCurrent, int partInstanceId, int priceId) {
            this.TimeStamp = timeStamp;
            this.IsCurrent = isCurrent;
            this.PartInstanceId = partInstanceId;
            this.PriceId = priceId;
        }

        //Sets by Id
        public void Set(PriceLog priceLog) {
            this.PartInstanceId = priceLog.PartInstanceId;
            this.PriceId = priceLog.PriceId;
            this.TimeStamp = priceLog.TimeStamp;
            this.IsCurrent = priceLog.IsCurrent;
        }

        public void Set(PartInstance partInstance, Price price,DateTime timeStamp,bool isCurrent) {
            this.PartInstance = partInstance;
            this.Price = price;
            this.TimeStamp = timeStamp;
            this.IsCurrent = isCurrent;
        }
    }
}
