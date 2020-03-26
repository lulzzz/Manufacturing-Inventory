﻿using ManufacturingInventory.Domain.DTOs;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.CategoryBoundaries {
    public enum CategoryOption {
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/SvgImages/Business Objects/BO_Quote.svg"), Display(Name = "Condition", Description = "Creates a Condition Category", Order = 1)]
        Condition,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/Images/Business Objects/BOProductGroup_16x16.png"), Display(Name = "StockType", Description = "Creates a StockType Category", Order = 2)]
        StockType,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/Images/XAF/BO_Organization.png"), Display(Name = "Organization", Description = "Creates a Organization Category", Order = 3)]
        Organization,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/SvgImages/Business Objects/BO_Vendor.svg"), Display(Name = "Usage", Description = "Creates a Usage Category", Order = 4)]
        Usage,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/Images/Actions/Cancel_16x16.png"), Display(Name = "Not Selected", Description = "Must Make a Selection", Order = 5)]
        NotSelected
    }

    public class CategoryBoundaryInput {
        public CategoryBoundaryInput(EditAction editAction, CategoryDTO category) {
            this.EditAction = editAction;
            this.Category = category;
        }

        public EditAction EditAction { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
