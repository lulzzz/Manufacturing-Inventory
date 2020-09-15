using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.Boundaries.LocationManage {
    public class LocationManagmentInput {
        public EditAction EditAction { get; set; }
        public LocationDto Location { get; set; }
        
        public LocationManagmentInput(LocationDto location,EditAction editAction) {
            this.EditAction = editAction;
            this.Location = location;
        }
    }
}
