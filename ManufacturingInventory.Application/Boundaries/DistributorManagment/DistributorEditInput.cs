using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.DistributorManagment {
    public class DistributorEditInput {

        public int? DistributorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EditAction EditAction { get; set; }

        //Delete
        public DistributorEditInput(int? distributorId, EditAction editAction) {
            this.DistributorId = distributorId;
            this.EditAction = editAction;
        }

        //update
        public DistributorEditInput(int? distributorId, string name, string description, EditAction editAction) {
            this.DistributorId = distributorId;
            this.Name = name;
            this.Description = description;
            this.EditAction = editAction;
        }

        //new distribur
        public DistributorEditInput(string name, string description, EditAction editAction) {
            this.DistributorId = null;
            this.Name = name;
            this.Description = description;
            this.EditAction = editAction;
        }
    }
}
