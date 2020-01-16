﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Checkout {
  
    public class CheckOutInputData {

        public CheckOutInputData(DateTime timeStamp, int partInstanceId, int locationId, int quantity, double unitCost, double totalCost) {
            this.TimeStamp = timeStamp;
            this.Quantity = quantity;
            this.UnitCost = unitCost;
            this.TotalCost = totalCost;
            this.PartInstanceId = partInstanceId;
            this.LocationId = locationId;
        }

        public DateTime TimeStamp { get; set; }
        public int Quantity { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }

        public int PartInstanceId { get; set; }
        public int LocationId { get; set; }

    }

    public class CheckOutBubblerInputData : CheckOutInputData {


        public CheckOutBubblerInputData(DateTime timeStamp, int partInstanceId, int locationId, int quantity, double unitCost, double totalCost,double measuredWeight,double weight) 
            : base(timeStamp, partInstanceId, locationId, quantity, unitCost, totalCost) {
            
        }

        public double Weight { get; set; }
        public double MeasuredWeight { get; set; }
    }
}
