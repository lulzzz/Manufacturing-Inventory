using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class ReportSnapshot {
        public string PartName { get; set; }
        public string InstanceName { get; set; }

        public double StartQuantity { get; set; }
        public double StartCost { get; set; }

        public double IncomingQuantity { get; set; }
        public double IncomingCost { get; set; }

        public double ProductionOutgoingQuantity { get; set; }
        public double ProductionOutgoingCost { get; set; }

        public double RndOutgoingQuantity { get; set; }
        public double RndOutgoingCost { get; set; }

        public double TotalOutgoingQuantity { get; set; }
        public double TotalOutgoingCost { get; set; }

        public double CurrentQuantity { get; set; }
        public double CurrentCost { get; set; }

        public double EndQuantity { get; set; }
        public double EndCost { get; set; }

    }
}
