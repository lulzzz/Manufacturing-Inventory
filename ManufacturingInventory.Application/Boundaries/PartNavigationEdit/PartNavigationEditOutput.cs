using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PartNavigationEdit {
    public class PartNavigationEditOutput : IOutput {
        public PartNavigationEditOutput(Part part, bool success, string message) {
            this.Part = part;
            this.Success = success;
            this.Message = message;
        }

        public Part Part { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
