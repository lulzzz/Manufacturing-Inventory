using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {
    public class AttachmentContext {
        public int PartId { get; set; }
        public ObservableCollection<Attachment> Attachments { get; set; }
    }
}
