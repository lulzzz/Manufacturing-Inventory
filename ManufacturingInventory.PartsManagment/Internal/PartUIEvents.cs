using ManufacturingInventory.Infrastructure.Model.Entities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {

    public class ReloadEventTraveler {
        public int PartId { get; set; }
        public int PartInstanceId { get; set; }

        public ReloadEventTraveler() {

        }

        public ReloadEventTraveler(int partId,int partInstanceId) {
            this.PartId = partId;
            this.PartInstanceId = partInstanceId;
        }
    }


    public class RenameHeaderEvent : PubSubEvent<string> { }



    //PartSummary-Edit
    public class PartEditDoneEvent:PubSubEvent<int> { }
    public class PartEditCancelEvent : PubSubEvent { }
    
    //Price-Edit
    public class PriceEditDoneEvent: PubSubEvent { }
    public class PriceEditCancelEvent : PubSubEvent { }

    //PartInstance-Edit
    public class PartInstanceEditDoneEvent : PubSubEvent<ReloadEventTraveler> { }
    public class PartInstanceEditCancelEvent : PubSubEvent<ReloadEventTraveler> { }

    //PartInstance-Outgoing
    public class OutgoingDoneEvent : PubSubEvent<int> { }
    public class AddToOutgoingEvent : PubSubEvent<PartInstance> { }
    public class OutgoingCancelEvent : PubSubEvent { }

    //PartInstance-CheckIn
    public class CheckInDoneEvent : PubSubEvent<int> { }
    public class CheckInCancelEvent : PubSubEvent { }

    //Transaction
    public class ReturnDoneEvent : PubSubEvent<int> { }
    public class ReturnCancelEvent : PubSubEvent { }

    //DoneAndView
    public class ViewModifiedInstanceEvent : PubSubEvent<ReloadEventTraveler> {

    }
}
