using Prism.Mvvm;

namespace ManufacturingInventory.Main.ViewModels {
    public class MainWindowViewModel : BindableBase {
        private string _title = "Manufacturing Inventory";
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel() {

        }
    }
}
