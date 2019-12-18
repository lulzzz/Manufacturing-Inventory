using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Model.Entities {
    public class BubblerParameter {
        public int Id { get; set; }
        public double NetWeight { get; set; }
        public double Tare { get; set; }
        public double GrossWeight { get; set; }
        public double Measured { get; set; }
        public double Weight { get; set; }

        public DateTime? DateInstalled { get; set; }
        public DateTime? DateRemoved { get; set; }

        public PartInstance PartInstance { get; set; }

        public BubblerParameter() {

        }

        public BubblerParameter(double net,double gross,double tare) {
            this.GrossWeight = gross;
            this.NetWeight = net;
            this.Tare = tare;
        }

        public BubblerParameter(PartInstance instance,double net, double gross, double tare) {
            this.GrossWeight = gross;
            this.NetWeight = net;
            this.Tare = tare;
            this.PartInstance = instance;
        }
    }
}
