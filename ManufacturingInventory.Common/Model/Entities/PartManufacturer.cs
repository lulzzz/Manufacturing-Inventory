using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Model.Entities {
    public class PartManufacturer {
        public int Id { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
    }
}
