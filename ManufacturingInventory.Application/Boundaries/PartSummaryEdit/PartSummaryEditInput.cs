using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.PartDetails {
    public class PartSummaryEditInput:IInput {

        //public PartSummaryEditInput(int partId, string name, string description, bool isNew,bool holdsBubblers,bool costReportedDefault, int? warehouseId, int? organizationId, int? usageId) {
        //    this.PartId = partId;
        //    this.Name = name;
        //    this.Description = description;
        //    this.IsNew = isNew;
        //    this.HoldsBubblers = holdsBubblers;
        //    this.CostReportedDefault = costReportedDefault;
        //    this.WarehouseId = warehouseId;
        //    this.OrganizationId = organizationId;
        //    this.UsageId = usageId;
        //}

        public PartSummaryEditInput(int partId, string name, string description, bool isNew, bool holdsBubblers, bool costReportedDefault, int? warehouseId, int? organizationId) {
            this.PartId = partId;
            this.Name = name;
            this.Description = description;
            this.IsNew = isNew;
            this.HoldsBubblers = holdsBubblers;
            this.CostReportedDefault = costReportedDefault;
            this.WarehouseId = warehouseId;
            this.OrganizationId = organizationId;
            //this.UsageId = usageId;
        }

        public bool IsNew { get; set; }
        public int PartId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HoldsBubblers { get; set; }
        public bool CostReportedDefault { get; set; }
        public int? WarehouseId { get; set; }
        public int? OrganizationId { get; set; }
        //public int? UsageId { get; set; }

    }
}
