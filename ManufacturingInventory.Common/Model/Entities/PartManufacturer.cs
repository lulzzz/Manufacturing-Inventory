using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Model.Entities {
    public class PartManufacturer {
        public int Id { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
    }

    //public class PartDistributor {
    //    public int Id { get; set; }

    //    public int PartId { get; set; }
    //    public Part Part { get; set; }

    //    public int DistributorId { get; set; }
    //    public Distributor Distributor { get; set; }
    //}

    //public class PartInstanceDistributor {
    //    public int Id { get; set; }

    //    public int PartInstanceId { get; set; }
    //    public PartInstance PartInstance { get; set; }

    //    public int DistributorId { get; set; }
    //    public Distributor Distributor { get; set; }
    //}
}
