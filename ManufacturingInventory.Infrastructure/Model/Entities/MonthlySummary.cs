using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class MonthlySummary {

        public MonthlySummary() {
            this.MonthlyPartSnapshots = new ObservableCollection<PartMonthlySummary>();
        }

        public MonthlySummary(DateTime monthStartDate, DateTime monthStopDate) {
            this.MonthStartDate = monthStartDate;
            this.MonthStopDate = monthStopDate;
            this.DateGenerated = DateTime.Now;
            this.MonthlyPartSnapshots = new ObservableCollection<PartMonthlySummary>();
        }

        public int Id { get; set; }
        public DateTime DateGenerated { get; set; }
        public DateTime MonthStartDate { get; set; }
        public DateTime MonthStopDate { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<PartMonthlySummary> MonthlyPartSnapshots { get; set; }


    }
}
