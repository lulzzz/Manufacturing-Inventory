using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {
    public class DataTraveler {
        public int PartId { get; set; }
        public bool HoldsBubblers { get; set; }
        public bool IsNew { get; set; }
        public bool IsEdit { get; set; }
    }
}
