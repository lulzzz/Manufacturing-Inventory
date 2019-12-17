using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {


    public class PartInstance {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SkuNumber { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }
        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public bool CostReported { get; set; }
        public bool IsResuable { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int? PartTypeId { get; set; }
        public PartType PartType { get; set; }

        public int? ConditionId { get; set; }
        public Condition Condition { get; set; }

        public int LocationId { get; set; }
        public Location CurrentLocation { get; set; }

        public Price Price { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public PartInstance() {
            this.Transactions = new ObservableHashSet<Transaction>();
            this.Attachments = new ObservableHashSet<Attachment>();
        }

        public PartInstance(Part part, string name, string serialNumber, string batchNumber, string skuNumber) : this() {
            this.Name = name;
            this.SerialNumber = serialNumber;
            this.BatchNumber = batchNumber;
            this.SkuNumber = skuNumber;
            this.Part = part;
        }

        public PartInstance(string name, string serialNumber, string batchNumber, string skuNumber) : this() {
            this.Name = name;
            this.SerialNumber = serialNumber;
            this.BatchNumber = batchNumber;
            this.SkuNumber = skuNumber;
        }

        public virtual void UpdatePrice() {
            this.UnitCost = this.Price.UnitCost;
            this.TotalCost = this.UnitCost * this.Quantity;
        }

        public virtual void UpdatePrice(Price price) {
            this.Price = price;
            this.Price.PartInstanceId = this.Id;
            this.Price.PartInstance = this;
            this.UnitCost = this.Price.UnitCost;
            this.TotalCost = this.UnitCost * this.Quantity;
        }
    }

    public class Bubbler : PartInstance {
        public double NetWeight { get; set; }
        public double Tare { get; set; }
        public double GrossWeight { get; set; }

        public double Measured { get; set; }
        public double Weight { get; set; }

        public DateTime? DateInstalled { get; set; }
        public DateTime? DateRemoved { get; set; }

        public Bubbler(Part part, string name, string serialNumber, string batchNumber, string skuNumber,double net,double tare,double gross) : base(part, name, serialNumber, batchNumber, skuNumber) {
            this.NetWeight = net;
            this.Tare = tare;
            this.GrossWeight = gross;
        }

        public Bubbler(string name, string serialNumber, string batchNumber, string skuNumber, double net, double tare,double gross) : base(name,serialNumber,batchNumber,skuNumber) {
            this.NetWeight = net;
            this.Tare = tare;
            this.GrossWeight = gross;
        }

        public Bubbler() : base() {

        }

        public void UpdateWeight(double measured) {
            this.Measured = measured;
            this.Weight = this.NetWeight-(this.GrossWeight - this.Measured);
            this.UpdatePrice();
        }

        public override void UpdatePrice(Price price) {
            this.Price = price;
            this.Price.PartInstanceId = this.Id;
            this.Price.PartInstance = this;
            this.UnitCost= this.Price.UnitCost;
            this.TotalCost = this.Weight * this.Price.UnitCost;
        }

        public override void UpdatePrice() {
            this.UnitCost = this.Price.UnitCost;
            this.TotalCost = this.Weight * this.Price.UnitCost;
        }
    }
}
