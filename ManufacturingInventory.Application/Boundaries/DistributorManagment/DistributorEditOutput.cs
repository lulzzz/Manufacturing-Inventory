using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.DistributorManagment {
    public class DistributorEditOutput : IOutput {

        public bool Success { get; set; }
        public string Message { get; set; }
        public Distributor Distributor { get; set; }

        public DistributorEditOutput(Distributor distributor,bool success, string message) {
            this.Success = success;
            this.Message = message;
            this.Distributor = distributor;
        }

        public DistributorEditOutput() {
            this.Success = false;
            this.Message = string.Empty;
            this.Distributor = null;
        }
    }
}
