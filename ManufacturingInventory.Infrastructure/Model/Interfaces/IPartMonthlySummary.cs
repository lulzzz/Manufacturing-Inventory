using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Infrastructure.Model.Interfaces {
    public interface IPartMonthlySummary {
        string PartName { get; set; }
        string InstanceName { get; set; }

        double StartQuantity { get; set; }
        double StartCost { get; set; }

        double IncomingQuantity { get; set; }
        double IncomingCost { get; set; }

        double ProductionOutgoingQuantity { get; set; }
        double ProductionOutgoingCost { get; set; }

        double RndOutgoingQuantity { get; set; }
        double RndOutgoingCost { get; set; }

        double TotalOutgoingQuantity { get; set; }
        double TotalOutgoingCost { get; set; }

        double CurrentQuantity { get; set; }
        double CurrentCost { get; set; }

        double EndQuantity { get; set; }
        double EndCost { get; set; }
    }
}
