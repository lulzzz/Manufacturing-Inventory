using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ManufacturingInventory.Infrastructure.Model.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Domain.Extensions;

namespace ManufacturingInventory.Domain.DTOs {
    public class CategoryDTO : ICategory {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int SafeQuantity { get; set; }
        public CategoryTypes Type { get; set; }


        public CategoryDTO(ICategory category) {
            this.Id = category.Id;
            this.Name =category.Name;
            this.Description = category.Description;
            this.IsDefault = category.IsDefault;
            this.Type = category.GetType().Name.GetEnum<CategoryTypes>(CategoryTypes.InvalidType);
            if (this.Type == CategoryTypes.StockType) {
                this.Quantity = ((StockType)category).Quantity;
                this.MinQuantity= ((StockType)category).MinQuantity;
                this.SafeQuantity= ((StockType)category).SafeQuantity;
            }
        }

        public Category GetCategory() {
            switch (this.Type) {
                case CategoryTypes.Organization:
                    return new Organization() { Name = this.Name, Description = this.Description, IsDefault = this.IsDefault };
                case CategoryTypes.Condition:
                    return new Condition() { Name = this.Name, Description = this.Description, IsDefault = this.IsDefault };
                case CategoryTypes.StockType:
                    return new StockType() { 
                        Name = this.Name, 
                        Description = this.Description, 
                        IsDefault = this.IsDefault,
                        Quantity=this.Quantity,
                        MinQuantity=this.MinQuantity,
                        SafeQuantity=this.SafeQuantity
                    };
                case CategoryTypes.Usage:
                    return new Usage() { Name = this.Name, Description = this.Description, IsDefault = this.IsDefault };
                case CategoryTypes.InvalidType:
                    return null;
                default:
                    return null;
            }
        }

        public CategoryDTO() {
            this.Type = CategoryTypes.InvalidType;
        }

        public CategoryDTO(string name, string description, bool isDefault, CategoryTypes type) {
            this.Name = name;
            this.Description = description;
            this.IsDefault = isDefault;
            this.Type = type;
        }

        public CategoryDTO(string name, string description, bool isDefault, int quantity, int minQuantity, int safeQuantity, CategoryTypes type) {
            this.Name = name;
            this.Description = description;
            this.IsDefault = isDefault;
            this.Quantity = quantity;
            this.MinQuantity = minQuantity;
            this.SafeQuantity = safeQuantity;
            this.Type = type;
        }
    }
}
