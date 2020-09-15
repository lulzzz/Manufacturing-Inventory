using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ManufacturingInventory.Domain.Enums {
    //public enum LocationTypes {
    //    [Description("Warehouse")] Warehouse,
    //    [Description("Consumer")] Consumer
    //}

    public enum LocationType {
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/Images/Business Objects/BOCountry_32x32.png"), Display(Name = "Warehouse", Description = "Create a Warehouse Type Location", Order = 1)]
        Warehouse,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/Images/Business Objects/BOProductGroup_32x32.png"), Display(Name = "Consumer", Description = "Creates a Consumer Type Location", Order = 2)]
        Consumer,
        [Image("pack://application:,,,/DevExpress.Images.v19.1;component/Images/Actions/Cancel_16x16.png"), Display(Name = "Not Selected", Description = "Must Make a Selection", Order = 3)]
        NotSelected
    }
}
