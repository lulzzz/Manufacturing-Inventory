using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit {
    public class PartInstanceDetailsEditInput {

        public PartInstanceDetailsEditInput(PartInstance partInstance) {
            this.PartInstance = partInstance;
        }

        public PartInstance PartInstance { get; set; }
    }
}
