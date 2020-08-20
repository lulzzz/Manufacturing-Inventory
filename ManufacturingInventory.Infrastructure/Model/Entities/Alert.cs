using System.Collections.Generic;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public abstract class Alert {
        public int Id { get; set; }
        public string AlertType { get; set; }
        public byte[] RowVersion { get; set; }
        public ICollection<UserAlert> UserAlerts { get; set; }

        public Alert() {
            this.UserAlerts = new HashSet<UserAlert>();
        }
    }

    public class CombinedAlert : Alert {
        public StockType StockHolder { get; set; }

        public CombinedAlert():base() {
            
        }
    }

    public class IndividualAlert : Alert {

        //public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public IndividualAlert() : base() {

        }

    }
}
