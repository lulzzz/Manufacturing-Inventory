using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ManufacturingInventory.Common.Application.UI.ControlEx {
    public class ComboBoxEditEx : ComboBoxEdit {
        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            if (e.Key == Key.Delete || e.Key == Key.Back) {
                SetCurrentValue(EditValueProperty, null);
                e.Handled = true;
            } else
                base.OnPreviewKeyDown(e);
        }
    }
}
