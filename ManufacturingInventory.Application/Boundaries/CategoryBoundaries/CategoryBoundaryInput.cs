using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CategoryBoundaries {
    public class CategoryBoundaryInput {
        public CategoryBoundaryInput(EditAction editAction, CategoryDTO category) {
            this.EditAction = editAction;
            this.Category = category;
        }

        public EditAction EditAction { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
