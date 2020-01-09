using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {

    public class PartInstance:ICloneable {
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
        public bool IsBubbler { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int? PartTypeId { get; set; }
        public PartType PartType { get; set; }

        public int? ConditionId { get; set; }
        public Condition Condition { get; set; }

        public int LocationId { get; set; }
        public Location CurrentLocation { get; set; }

        public int? BubblerParameterId { get; set; }
        public BubblerParameter BubblerParameter { get; set; }

        public Price Price { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public PartInstance() {
            this.Transactions = new ObservableHashSet<Transaction>();
            this.Attachments = new ObservableHashSet<Attachment>();
        }

        public PartInstance(Part part, string name, string serialNumber, string batchNumber, string skuNumber,bool isBubbler) : this() {
            this.Name = name;
            this.SerialNumber = serialNumber;
            this.BatchNumber = batchNumber;
            this.SkuNumber = skuNumber;
            this.Part = part;
            this.IsBubbler = isBubbler;
        }

        public PartInstance(string name, string serialNumber, string batchNumber, string skuNumber, bool isBubbler) : this() {
            this.Name = name;
            this.SerialNumber = serialNumber;
            this.BatchNumber = batchNumber;
            this.SkuNumber = skuNumber;
            this.IsBubbler = isBubbler;
        }

        public PartInstance(Part part, string name, string serialNumber, string batchNumber, string skuNumber, bool isBubbler, BubblerParameter param) : this(part, name, serialNumber, batchNumber, skuNumber,isBubbler) {
            this.BubblerParameter = param;
        }

        public PartInstance(string name, string serialNumber, string batchNumber, string skuNumber, bool isBubbler, BubblerParameter param) : this(name, serialNumber, batchNumber, skuNumber,isBubbler) {
            this.BubblerParameter = param;
        }

        public void UpdateWeight(double measured) {        
            this.BubblerParameter.Measured = measured;
            this.BubblerParameter.Weight = this.BubblerParameter.NetWeight - (this.BubblerParameter.GrossWeight - this.BubblerParameter.Measured);
        }

        public void UpdatePrice() {
            if (!this.IsBubbler) {
                this.UnitCost = this.Price.UnitCost;
                this.TotalCost = this.UnitCost * this.Quantity;
            } else {
                this.UnitCost = this.Price.UnitCost;
                this.TotalCost = this.UnitCost * this.BubblerParameter.NetWeight;
            }
        }

        public void UpdatePrice(Price price) {
            if (!this.IsBubbler) {
                this.Price = price;
                this.Price.PartInstanceId = this.Id;
                this.Price.PartInstance = this;
                this.UnitCost = this.Price.UnitCost;
                this.TotalCost = this.UnitCost * this.Quantity;
            } else {
                this.Price = price;
                this.Price.PartInstanceId = this.Id;
                this.Price.PartInstance = this;
                this.UnitCost = this.Price.UnitCost;
                this.TotalCost = this.UnitCost * this.BubblerParameter.NetWeight;
            }
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
