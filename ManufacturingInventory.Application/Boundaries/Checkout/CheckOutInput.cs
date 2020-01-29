using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Checkout {

    //public class CheckOutStandardInput : ICheckOutInput<CheckOutInputData> {
    //    public IList<CheckOutInputData> Items { get; set; } = new List<CheckOutInputData>();
    //}

    public class CheckOutInput : ICheckOutInput<CheckOutInputData> {
        public IList<CheckOutInputData> Items { get; set; } = new List<CheckOutInputData>();
    }
}
