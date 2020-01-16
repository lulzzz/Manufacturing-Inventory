using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Checkout {
    public class CheckOutOutputData {

        public CheckOutOutputData(Transaction transaction, bool success, string message) {
            this.Transaction = transaction;
            this.Success = success;
            this.Message = message;
        }

        public Transaction Transaction { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
