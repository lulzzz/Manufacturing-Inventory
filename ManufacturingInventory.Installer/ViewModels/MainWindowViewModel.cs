using Prism.Mvvm;

namespace ManufacturingInventory.Installer.ViewModels {
    public class MainWindowViewModel : BindableBase {
        private string _title = "Inventory Installer";
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel() {

        }
    }
}
