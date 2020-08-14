using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ManufacturingInventory.Domain.Enums {
    public enum CollectType {
        [Display(Name = "Cost Reported Only", Description = "Only Collects Items with CostReported Field Selected", Order = 1)]
        OnlyCostReported,
        [Display(Name = "All Items", Description = "Collects All Items", Order = 2)]
        AllItems,
        [Display(Name = "Cost Not Reported Only", Description = "Only Collects Items with CostReported Field Not Selected", Order = 3)]
        OnlyCostNotReported
    }
}
