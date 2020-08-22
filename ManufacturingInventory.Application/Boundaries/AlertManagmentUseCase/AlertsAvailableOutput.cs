using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {
    public class AlertsAvailableOutput : IOutput {
        public Alert Alert { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public AlertsAvailableOutput(Alert alert,bool success,string message) {
            this.Alert = alert;
            this.Success = success;
            this.Message = message;
        }
    }
}
