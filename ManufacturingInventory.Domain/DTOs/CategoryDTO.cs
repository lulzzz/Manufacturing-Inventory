using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Domain.DTOs {
    public class CategoryDTO : ICategory {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public string Type { get; set; }


        public CategoryDTO(ICategory category) {
            this.Id = category.Id;
            this.Name =category.Name;
            this.Description = category.Description;
            this.IsDefault = category.IsDefault;
            this.Type = category.GetType().Name;
        }

        public CategoryDTO() {

        }
    }
}
