using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces {
    public interface IAttachmentEditOutput<T>:IOutput {
        T Entity { get; set; }
    }
}
