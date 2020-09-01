using ManufacturingInventory.Infrastructure.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public class PartSummary:IPartSummary {
        public string PartName { get; set; }
        public string InstanceName { get; set; }
        public DateTime Today { get; set; }
        public DateTime DateIn { get; set; }
        public int Age { get; set; }

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

        public PartSummary() {

        }

        public PartSummary(IPartSummary partSummary) {
            this.PartName = partSummary.PartName;
            this.InstanceName = partSummary.InstanceName;
            this.StartQuantity = partSummary.StartQuantity;
            this.StartCost = partSummary.StartCost;
            this.IncomingQuantity = partSummary.IncomingQuantity;
            this.IncomingCost = partSummary.IncomingCost;
            this.ProductionOutgoingQuantity = partSummary.ProductionOutgoingQuantity;
            this.ProductionOutgoingCost = partSummary.ProductionOutgoingCost;
            this.RndOutgoingQuantity = partSummary.RndOutgoingQuantity;
            this.RndOutgoingCost = partSummary.RndOutgoingCost;
            this.TotalOutgoingQuantity = partSummary.TotalOutgoingQuantity;
            this.TotalOutgoingCost = partSummary.TotalOutgoingCost;
            this.CurrentQuantity = partSummary.CurrentQuantity;
            this.CurrentCost = partSummary.CurrentCost;
            this.EndQuantity = partSummary.EndQuantity;
            this.EndCost = partSummary.EndCost;
        }

    }
}
