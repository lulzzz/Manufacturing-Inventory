using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
    public class ProgressTraveler {
        public string LogLine { get; set; }
    }
    
    public class CancelEvent:PubSubEvent { }
    public class FinishedEvent : PubSubEvent { }
    public class IncrementProgress : PubSubEvent<string> { }
    
}
