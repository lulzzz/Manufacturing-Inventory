using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;

namespace ManufacturingInventory.LocationManagment.Internal {
    public class ReloadEvent: PubSubEvent { }
    public class LocationEditDoneEvent : PubSubEvent<int> { }
}
