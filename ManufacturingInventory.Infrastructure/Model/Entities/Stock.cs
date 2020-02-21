using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public abstract class Stock {
        public int Id { get; set; }
        public int Quanitity { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

    }

    public class StandardStock : Stock {
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
    }

    public class BubblerStock : Stock {
        public double Weight { get; set; }
        public double Net { get; set; }
        public double Gross { get; set; }
    }

    public class PartStock {
        public int Id { get; set; }

        public ICollection<PartInstance> PartInstances { get; set; }

    }
}
