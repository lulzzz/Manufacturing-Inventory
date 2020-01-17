using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries {
    public interface IInput {
        bool IsNew { get; set; }
    }
}
