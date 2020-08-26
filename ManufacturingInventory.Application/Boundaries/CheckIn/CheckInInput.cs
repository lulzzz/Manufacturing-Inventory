using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DevExpress.Mvvm.DataAnnotations;


namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public enum PriceOption {
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/DevAV/Actions/NewSales_16x16.png"), Display(Name = "Create New Price", Description = "Creates New Price", Order = 1)]
        CreateNew,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/DevAV/Actions/CostAnalysis_16x16.png"), Display(Name = "Select Existing Price", Description = "Use Existing Price", Order = 1)]
        UseExisting,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/DevAV/Actions/Close_16x16.png"), Display(Name = "No Price", Description = "Check In Without Price", Order = 3)]
        NoPrice
    }

    public class CheckInInput {

        public CheckInInput(PartInstance partInstance, PriceOption priceOption, DateTime timeStamp,int partId,bool isExisiting,int? quantity=null,Price price=null) {
            this.PartInstance = partInstance;
            this.TimeStamp = timeStamp;
            this.PricingOption = priceOption;
            this.Price = price;
            this.PartId = partId;
            this.Quantity = quantity;
            this.IsExisiting = isExisiting;
        }

        public PartInstance PartInstance { get; set; }
        public int PartId { get; set; }
        public Price Price { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsExisiting { get; set; }
        public int? Quantity { get; set; }
        public PriceOption PricingOption { get; set; }
    }
}
