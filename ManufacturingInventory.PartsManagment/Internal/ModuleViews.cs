using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {
    public static class ModuleViews {
        public static string PartInstanceDetailsView { get => "PartInstanceDetailsView"; }
        public static string PartsDetailView { get => "PartsDetailView"; }
        public static string PartsManagmentMainView { get => "PartsManagmentMainView"; }
        public static string PartsNavigationView { get => "PartsNavigationView"; }
        public static string CheckoutView { get => "CheckoutView"; }
        public static string CheckInView { get => "CheckInView"; }
        public static string ReturnItemView { get => "ReturnItemView"; }

        public static string PriceDetailsView { get => "PriceDetailsView"; }
        public static string SelectPriceView { get => "SelectPriceView"; }
    }
}
