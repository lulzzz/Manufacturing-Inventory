using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.DistributorManagment.Internal {
    public class DistributorEditDoneEvent : PubSubEvent<int?> { }
    public class DistributorEditCancelEvent : PubSubEvent<int?> { }
    public class ReloadNoClearEvent : PubSubEvent { }
}
