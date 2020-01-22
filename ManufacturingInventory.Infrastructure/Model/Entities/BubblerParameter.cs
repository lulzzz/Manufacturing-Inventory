using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
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
            this.GrossWeight = 0;
            this.NetWeight = 0;
            this.Tare = 0;
            this.Weight = 0;
            this.Measured = 0;
        }

        public BubblerParameter(double net,double gross,double tare) {
            this.GrossWeight = gross;
            this.NetWeight = net;
            this.Tare = tare;
            this.Measured = 0;
            this.Weight = 0;
        }

        public BubblerParameter(PartInstance instance,double net, double gross, double tare) {
            this.GrossWeight = gross;
            this.NetWeight = net;
            this.Tare = tare;
            this.PartInstance = instance;
            this.Measured = 0;
            this.Weight = 0;
        }

        public void UpdateWeight(double measured) {
            this.Measured = measured;
            this.Weight = this.NetWeight - (this.GrossWeight - this.Measured);
        }

        public void UpdateWeight() {
            this.Weight = this.NetWeight - (this.GrossWeight - this.Measured);
        }

        public void Set(BubblerParameter bubbler) {
            this.Weight = bubbler.Weight;
            this.NetWeight = bubbler.NetWeight;
            this.Tare = bubbler.Tare;
            this.GrossWeight = bubbler.GrossWeight;
            this.Measured =bubbler.Measured;
            this.DateInstalled = bubbler.DateInstalled;
            this.DateRemoved = bubbler.DateRemoved;
            this.UpdateWeight();
        }

        public bool Compare(BubblerParameter rhs) {
            return (this.NetWeight == rhs.NetWeight) 
                && (this.GrossWeight == rhs.GrossWeight) 
                && (this.Weight == rhs.Weight) 
                && (this.Measured==rhs.Measured) 
                && (this.Tare == rhs.Tare) 
                && (this.DateInstalled == rhs.DateInstalled) 
                && (this.DateRemoved == rhs.DateRemoved);
        }


    }
}
