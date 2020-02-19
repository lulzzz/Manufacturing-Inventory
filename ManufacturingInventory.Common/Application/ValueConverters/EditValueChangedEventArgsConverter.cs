using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Editors;
using ManufacturingInventory.Application.Boundaries.CheckIn;

namespace ManufacturingInventory.Common.Application.ValueConverters {
    public class EditValueChangedEventArgsConverter : EventArgsConverterBase<EditValueChangedEventArgs> {
        protected override object Convert(object sender, EditValueChangedEventArgs args) {
            //var element = LayoutTreeHelper.GetVisualParents((DependencyObject)args.,(DependencyObject)sender).OfType<ComboBoxEdit>().FirstOrDefault();
            return args.OldValue != null ? (PriceOption)args.OldValue : PriceOption.NoPrice;
        }
    }
}
