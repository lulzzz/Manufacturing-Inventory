﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PartDetails {
    public class PartSummaryInput {

        public PartSummaryInput(int partId, string name, string description, bool holdsBubblers, int? warehouseId, int? organizationId, int? usageId) {
            this.PartId = partId;
            this.Name = name;
            this.Description = description;
            this.HoldsBubblers = holdsBubblers;
            this.WarehouseId = warehouseId;
            this.OrganizationId = organizationId;
            this.UsageId = usageId;
        }

        public int PartId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HoldsBubblers { get; set; }
        public int? WarehouseId { get; set; }
        public int? OrganizationId { get; set; }
        public int? UsageId { get; set; }
    }
}
