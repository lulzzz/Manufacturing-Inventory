using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries {
    public interface IOutput {
        bool Success { get; set; }
        string Message { get; set; }
    }
}
