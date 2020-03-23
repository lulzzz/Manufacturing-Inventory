using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CategoryBoundaries {
    public class CategoryBoundaryOutput : IOutput {
        public Category Category;

        public CategoryBoundaryOutput(Category category, bool success, string message) {
            this.Category = category;
            this.Success = success;
            this.Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
