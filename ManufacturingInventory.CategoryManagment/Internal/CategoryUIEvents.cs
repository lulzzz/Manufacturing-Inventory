using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.CategoryManagment.Internal {
    public class CategoryEditDoneEvent : PubSubEvent<int?> { }
    public class CategoryEditCancelEvent : PubSubEvent<int?> { }
    public class ReloadNoClearEvent : PubSubEvent { }
}