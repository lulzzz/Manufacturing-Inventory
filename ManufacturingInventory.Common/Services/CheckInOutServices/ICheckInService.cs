using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Services {
    public interface ICheckInService {
        PartInstance CheckInBubbler(InventoryAction action,double measured);
        PartInstance CheckIn(InventoryAction action, int quantity);
    }
}
