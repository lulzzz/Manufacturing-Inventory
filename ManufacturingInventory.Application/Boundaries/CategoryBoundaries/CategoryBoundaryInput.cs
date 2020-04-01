using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.Boundaries.CategoryBoundaries {


    public class CategoryBoundaryInput {
        public CategoryBoundaryInput(EditAction editAction, CategoryDTO category, bool isDefaultChanged = false) {
            this.EditAction = editAction;
            this.Category = category;
            this.IsDefault_Changed = isDefaultChanged;
        }

        public EditAction EditAction { get; set; }
        public CategoryDTO Category { get; set; }
        public bool IsDefault_Changed;
    }
}
