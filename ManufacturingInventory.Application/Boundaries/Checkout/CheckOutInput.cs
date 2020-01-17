﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Checkout {

    public class CheckOutStandardInput : ICheckOutInput<CheckOutInputData> {
        public IList<CheckOutInputData> Items { get; set; }
    }

    public class CheckOutBubblerInput : ICheckOutInput<CheckOutBubblerInputData> {
        public IList<CheckOutBubblerInputData> Items { get; set; }
    }
}
