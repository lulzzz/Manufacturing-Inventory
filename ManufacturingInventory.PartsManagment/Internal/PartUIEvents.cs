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

    public class RenameHeaderEvent : PubSubEvent<string> { }

    public class ReloadEvent:PubSubEvent<ReloadEventTraveler> { }

    public class PartEditDoneEvent:PubSubEvent<int> { }
    public class PartEditCancelEvent : PubSubEvent { }

    public class PriceEditDoneEvent : PubSubEvent { }
    public class PriceEditCancelEvent : PubSubEvent { }
    public class InstanceCreatedEvent : PubSubEvent<int> { };

    public class PartInstanceEditDoneEvent : PubSubEvent { }
    public class LoadPartDetailsEvent : PubSubEvent { }

    public class OutgoingDoneEvent : PubSubEvent { }
    public class AddToOutgoingEvent : PubSubEvent<PartInstance> { }

    public class ReturnDoneEvent : PubSubEvent { }
}
