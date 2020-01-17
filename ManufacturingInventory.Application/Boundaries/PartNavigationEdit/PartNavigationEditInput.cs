using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PartNavigationEdit {
    public class PartNavigationEditInput  {
        public PartNavigationEditInput(int partId) => this.PartId = partId;

        public int PartId { get; set; }
    }
}
