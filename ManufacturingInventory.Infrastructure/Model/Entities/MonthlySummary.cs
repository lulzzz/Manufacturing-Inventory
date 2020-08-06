using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class MonthlySummary {

        public int Id { get; set; }
        public string MonthOfReport { get; set; }
        public DateTime DateGenerated { get; set; }
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthStopDate { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<PartMonthlySummary> MonthlyPartSnapshots { get; set; }

        public MonthlySummary() {
            this.MonthlyPartSnapshots = new HashSet<PartMonthlySummary>();
        }

        public MonthlySummary(DateTime monthStartDate, DateTime monthStopDate) {
            this.MonthStartDate = monthStartDate;
            this.MonthStopDate = monthStopDate;
            this.DateGenerated = DateTime.Now;
            this.MonthlyPartSnapshots = new HashSet<PartMonthlySummary>();
            this.MonthOfReport = this.DateGenerated.ToString("MMMM");
        }

        public void SetMonth(DateTime dateOfReport) {
            this.MonthOfReport = dateOfReport.ToString("MMMM");
        }

        public void Set(MonthlySummary monthlySummary) {
            this.MonthStartDate = monthlySummary.MonthStartDate;
            this.MonthStopDate = monthlySummary.MonthStopDate;
            this.MonthOfReport = monthlySummary.MonthOfReport;
            this.DateGenerated = monthlySummary.DateGenerated;
            this.MonthlyPartSnapshots = new HashSet<PartMonthlySummary>(monthlySummary.MonthlyPartSnapshots);
        }
    }
}
