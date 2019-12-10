using DevExpress.Mvvm;
using ManufacturingInventory.InstallSequence.Infrastructure;
using Prism.Events;
using Prism.Mvvm;

namespace ManufacturingInventory.Installer.ViewModels {
    public class MainWindowViewModel : InventoryViewModelBase {
        private IEventAggregator _eventAggregator;
        protected ICurrentWindowService CurrentWindowService { get => ServiceContainer.GetService<ICurrentWindowService>("CurrentWindowService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("MessageBoxService"); }

        private string _title = "Inventory Installer";
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public override bool KeepAlive {
            get => true;
        }

        public MainWindowViewModel(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<CancelEvent>().Subscribe(this.Cancel);
            this._eventAggregator.GetEvent<FinishedEvent>().Subscribe(this.Close);
        }

        private void Cancel() {
            var respose=this.MessageBoxService.ShowMessage("Are you sure want to quit?", "Warning", MessageButton.YesNo);
            if (respose == MessageResult.Yes) {
                this.CurrentWindowService.Close();
            }
        }

        private void Close() {
            this.CurrentWindowService.Close();
        }
    }
}
