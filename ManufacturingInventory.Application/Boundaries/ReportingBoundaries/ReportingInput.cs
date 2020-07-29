using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {
    public class ReportingInput {
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }

        public ReportingInput() {
            this.StartDate = DateTime.Now;
            this.StopDate = DateTime.Now;
        }

        public ReportingInput(DateTime start,DateTime stop) {
            this.StartDate = start;
            this.StopDate = stop;
        }
    }
}
