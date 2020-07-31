using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public interface IReportingInput {
        DateTime StartDate { get; set; }
        DateTime StopDate { get; set; }
    }

    public class MonthlySummaryInput: IReportingInput {
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }

        public MonthlySummaryInput() {
            this.StartDate = DateTime.Now;
            this.StopDate = DateTime.Now;
        }

        public MonthlySummaryInput(DateTime start,DateTime stop) {
            this.StartDate = start;
            this.StopDate = stop;
        }
    }

    public class TransactionSummaryInput {
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }
        public bool IncludeAll { get; set; }

        public TransactionSummaryInput() {
            this.StartDate = DateTime.Now;
            this.StopDate = DateTime.Now;
            this.IncludeAll = false;
        }

        public TransactionSummaryInput(DateTime start, DateTime stop,bool includeAll) {
            this.StartDate = start;
            this.StopDate = stop;
            this.IncludeAll = true;
        }
    }

    public class CurrentInventoryInput {
        public bool IncludeAll { get; set; }

        public CurrentInventoryInput(bool includeAll) {
            this.IncludeAll = includeAll;
        }

        public CurrentInventoryInput() {
            this.IncludeAll = false;
        }
    }
}
