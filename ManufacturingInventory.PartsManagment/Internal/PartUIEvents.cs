using ManufacturingInventory.Infrastructure.Model.Entities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {

    public class ReloadEventTraveler {
        public int PartId { get; set; }
        public int PartInstanceId { get; set; }
    }

    public class ReloadEvent:PubSubEvent<ReloadEventTraveler> { }
    public class PartEditDoneEvent:PubSubEvent { }
    public class PartInstanceEditDoneEvent : PubSubEvent { }
    public class LoadPartDetailsEvent : PubSubEvent { }

    public class OutgoingDoneEvent : PubSubEvent { }
    public class AddToOutgoingEvent : PubSubEvent<PartInstance> { }
}
