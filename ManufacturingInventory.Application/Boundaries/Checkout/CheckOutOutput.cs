using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Checkout {
    public class CheckOutOutput {

        public CheckOutOutput() {
            this.OutputList = new List<CheckOutOutputData>();
        }

        public IList<CheckOutOutputData> OutputList { get; set; }
    }
}
