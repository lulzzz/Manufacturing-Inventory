using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit {
    public class PartInstanceDetailsEditInput : IInput {

        public PartInstanceDetailsEditInput(PartInstance partInstance, bool isNew) {
            this.PartInstance = partInstance;
            this.IsNew = isNew;
        }

        public PartInstance PartInstance { get; set; }
        public bool IsNew { get; set; }
    }
}
