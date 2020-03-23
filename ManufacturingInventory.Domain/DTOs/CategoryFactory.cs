using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Domain.DTOs {
    public static class CategoryFactory {
        public static Category CreateCategory(CategoryDTO category) {
            if (typeof(Condition).Name == category.Type) {
                return null;
            } else if(typeof(Organization).Name == category.Type) {
                return null;
            } else if (typeof(Usage).Name == category.Type) {
                return null;
            } else if (typeof(StockType).Name == category.Type) {
                return null;
            } else {
                return null;
            }
        }
    }
}
