using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace ManufacturingInventory.Application.Boundaries.PriceEdit {
    public class PriceEditOutput : IOutput {

        public PriceEditOutput(Price price, bool success, string message) {
            this.Price = price;
            this.Success = success;
            this.Message = message;
        }

        public Price Price { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
