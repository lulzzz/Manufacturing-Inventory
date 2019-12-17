using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {
    public class PartEditDoneEvent:PubSubEvent { }
    public class PartInstanceEditDoneEvent : PubSubEvent { }

    public class LoadPartDetailsEvent : PubSubEvent { }
}
