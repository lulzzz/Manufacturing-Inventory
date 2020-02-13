using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CheckIn {
    public class CheckInOutput : IOutput {
        public PartInstance PartInstance { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
