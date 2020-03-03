using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace ManufacturingInventory.Infrastructure.Model.Entities {

    public class PartInstance:ICloneable {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string SkuNumber { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }
        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public bool CostReported { get; set; }
        public bool IsBubbler { get; set; }
        public bool IsReusable { get; set; }

        public DateTime? DateInstalled { get; set; }
        public DateTime? DateRemoved { get; set; }

        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int StockTypeId { get; set; }
        public StockType StockType { get; set; }

        public int? ConditionId { get; set; }
        public Condition Condition { get; set; }

        public int? UsageId { get; set; }
        public Usage Usage { get; set; }

        public int LocationId { get; set; }
        public Location CurrentLocation { get; set; }

        public int? BubblerParameterId { get; set; }
        public BubblerParameter BubblerParameter { get; set; }

        public int? PriceId { get; set; }
        public Price Price { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<PriceLog> PriceLogs { get; set; }

        public PartInstance() {
            this.Transactions = new HashSet<Transaction>();
            this.Attachments = new HashSet<Attachment>();
            this.PriceLogs = new HashSet<PriceLog>();
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
            this.BubblerParameter.UpdateWeight(measured);
            this.UpdatePrice();
        }

        public void UpdateWeight() {
            this.BubblerParameter.UpdateWeight();

            this.UpdatePrice();
        }

        public void UpdateQuantity(int delta) {
            this.Quantity += delta;
            this.UpdatePrice();
        }

        public void UpdatePrice() {
            if (!this.IsBubbler) {
                if (this.Price!=null) {
                    this.UnitCost = this.Price.UnitCost;
                    this.TotalCost = this.UnitCost * this.Quantity;
                }
            } else {
                if (this.Price != null) {
                    this.UnitCost = this.Price.UnitCost;
                    if (this.BubblerParameter != null) {
                        this.TotalCost = (this.UnitCost * this.BubblerParameter.NetWeight) * this.Quantity;
                    }
                }
            }
        }

        public void UpdatePrice(int priceId,double unitCost) {
            this.PriceId = priceId;
            if (!this.IsBubbler) {
                    this.UnitCost = unitCost;
                    this.TotalCost = this.UnitCost * this.Quantity;
            } else {
                this.UnitCost = unitCost;
                if (this.BubblerParameter != null) {
                    this.TotalCost = (this.UnitCost * this.BubblerParameter.NetWeight) * this.Quantity;
                }
            }
        }

        public void UpdatePrice(Price price) {
            if (!this.IsBubbler) {
                this.Price = price;
                this.UnitCost = price.UnitCost;
                this.TotalCost = this.UnitCost * this.Quantity;
            } else {
                this.Price = price;
                this.UnitCost = price.UnitCost;
                if (this.BubblerParameter != null) {
                    this.TotalCost = (this.UnitCost * this.BubblerParameter.NetWeight) * this.Quantity;
                }
            }
        }

        public void EditQuantity(int quantity) {
            if (!this.IsBubbler) {
                this.Quantity = quantity;
                this.TotalCost = this.UnitCost * this.Quantity;
            } else {
                this.Quantity = quantity;
                this.UnitCost = this.Price.UnitCost;
                if (this.BubblerParameter != null) {
                    this.TotalCost = (this.UnitCost * this.BubblerParameter.NetWeight) * this.Quantity;
                }
            }
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
