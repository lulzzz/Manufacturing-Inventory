using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {
    public class AlertUseCaseOutput : IOutput {
        public Alert Alert { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public AlertUseCaseOutput() {
            this.Alert = null;
            this.Success = false;
            this.Message = "Error: No Input Given";
        }

        public AlertUseCaseOutput(Alert alert,bool success,string message) {
            this.Alert = alert;
            this.Success = success;
            this.Message = message;
        }
    }
}
