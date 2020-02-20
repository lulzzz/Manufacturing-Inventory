using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Mvvm;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class LoadingViewModel:ViewModelBase {
        protected ICurrentWindowService CurrentWindowService { get { return this.GetService<ICurrentWindowService>(); } }

        public string Message {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public LoadingViewModel() {
            this.Message = "Loading, Please Wait...";
        }
    }
}
