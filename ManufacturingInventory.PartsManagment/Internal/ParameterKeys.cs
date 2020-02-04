using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {
    public static class ParameterKeys {
        public static string SelectedPart { get => "SelectedPart"; }
        public static string SelectedAttachment { get => "SelectedTransaction"; }
        public static string SelectedTransaction { get => "SelectedTransaction"; }
        public static string SelectedPartInstance{ get => "SelectedPartInstance"; }

        public static string InstanceId { get => "InstanceId"; }
        public static string PriceId { get=> "PriceId"; }
        public static string PartId { get => "PartId"; }

        public static string IsEdit { get => "IsEdit"; }
        public static string IsNew { get => "IsNew"; }
        public static string IsReload { get => "IsReload"; }
        public static string IsBubbler { get => "IsBubbler"; }

    }
}
