using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Common.Model.Entities;

namespace ManufacturingInventory.Common.Service {
    public interface ICreatePartInstanceService {
        PartInstance CreatePartInstance();
        PartInstance CreateBubbler();
    }
}
