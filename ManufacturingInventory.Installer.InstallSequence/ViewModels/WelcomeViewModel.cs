using System.Text;
using ManufacturingInventory.InstallSequence.Infrastructure;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;

namespace ManufacturingInventory.InstallSequence.ViewModels {
    public class WelcomeViewModel: InventoryViewModelNavigationBase {
        private IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;
        private IEventAggregator _eventAggregator;
        private VersionCheckerResponce _installTraveler;
        private bool _canContinue=true;
        private string _message;
        public PrismCommands.DelegateCommand NextCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }

        public WelcomeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,VersionCheckerResponce installTraveler) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._installTraveler = installTraveler;
            this.NextCommand = new PrismCommands.DelegateCommand(this.GoFoward,()=>this._canContinue);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.Cancel);
            this.BuilderMessage();
        }

        public override bool KeepAlive {
            get => false;
        }

        public string Message { 
            get => this._message; 
            set => SetProperty(ref this._message,value,"Message");
        }

        private void BuilderMessage() {
            StringBuilder builder = new StringBuilder();
            switch (this._installTraveler.InstallStatus) {
                case InstallStatus.InstalledNewVersion:
                    builder.AppendLine("New Version Available");
                    builder.AppendLine("Press Next To Update");
                    this.Message = builder.ToString();
                    this._canContinue = true;
                    break;
                case InstallStatus.InstalledUpToDate:
                    builder.AppendLine("Software is installed and up to date");
                    this.Message = builder.ToString();
                    this._canContinue = false;
                    break;
                case InstallStatus.NotInstalled:
                    builder.AppendLine("Press Next to continue");
                    this.Message = builder.ToString();
                    this._canContinue = true;
                    break;
                case InstallStatus.ServerFilesMissing:
                    builder.AppendLine("Error: Server files missing.  Please contact admin");
                    this.Message = builder.ToString();
                    this._canContinue = false;
                    break;
            }
        }

        private void GoFoward() {
            if (this._installTraveler.InstallStatus == InstallStatus.InstalledNewVersion) {
                var parameters = new NavigationParameters();
                parameters.Add("InstallLocation", Constants.InstallLocationDefault);
                this._regionManager.RequestNavigate("ContentRegion", "InstallingView", parameters);
            } else {
                this._regionManager.RequestNavigate("ContentRegion", "FileLocationView");
            }
            this._journal.GoForward();
        }


        private bool CanGoForward() {
            return this._journal != null && this._journal.CanGoForward;
        }

        private void Cancel() {
            this._eventAggregator.GetEvent<CancelEvent>().Publish();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._journal = navigationContext.NavigationService.Journal;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {
            
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

    }
}
