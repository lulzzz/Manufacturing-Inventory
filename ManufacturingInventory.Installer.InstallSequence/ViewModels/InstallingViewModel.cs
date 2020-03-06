using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using ManufacturingInventory.InstallSequence.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;


namespace ManufacturingInventory.InstallSequence.ViewModels {
    public class InstallingViewModel: InventoryViewModelNavigationBase {
        private string _installLocation;
        private string _installLog;
        private bool _isIndeterminate;
        private IRegionManager _regionManager;
        private IRegionNavigationJournal _journal;
        private IEventAggregator _eventAggregator;
        private IInstaller _installer;
        private VersionCheckerResponce _installTraveler;
        private int _itemCount;
        private int _maxProgress;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private bool _installing = false;
        private string _progressLabel;
        private string _buttonLabel;

        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DispatcherService"); }
        protected ICurrentWindowService CurrentWindowService { get => ServiceContainer.GetService<ICurrentWindowService>("InstallerCurrentWindow"); }
        protected IMessageBoxService MessageBox  { get => ServiceContainer.GetService<IMessageBoxService>("InstallerMessageBox"); }


        public PrismCommands.DelegateCommand BackCommand { get; private set; }
        public PrismCommands.DelegateCommand NextCommand { get; private set; }
        public PrismCommands.DelegateCommand CancelCommand { get; private set; }
        public AsyncCommand InstallCommand { get; private set; }

        public InstallingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,IInstaller installer,VersionCheckerResponce installTraveler) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._installer = installer;
            this._installTraveler=installTraveler;
            this.IsIndeterminate = false;
            this.ItemCount = 0;
            this.IsInstalling = false;
            this.MaxProgress = 100;
            this.ProgressLabel = "Uninstall Progress";
            this.ButtonLabel = "Cancel";
            this.NextCommand = new PrismCommands.DelegateCommand(this.GoForward);
            this.BackCommand = new PrismCommands.DelegateCommand(this.GoBack);
            this.CancelCommand = new PrismCommands.DelegateCommand(this.Cancel);
            this.InstallCommand = new AsyncCommand(this.InstallHandler,()=>!this._installing);
            this._eventAggregator.GetEvent<IncrementProgress>().Subscribe(async (logLine)=> await this.IncrementProgressHandler(logLine));
        }

        public override bool KeepAlive {
            get => false;
        }

        public bool IsInstalling {
            get => !this._installing;
            set => SetProperty(ref this._installing, value);
        }

        public string InstallLog { 
            get => this._installLog;
            set => SetProperty(ref this._installLog, value); 
        }
        public bool IsIndeterminate { 
            get => this._isIndeterminate;
            set => SetProperty(ref this._isIndeterminate, value);
        }
        public int ItemCount { 
            get => this._itemCount; 
            set => SetProperty(ref this._itemCount, value);
        }

        public int MaxProgress { 
            get => this._maxProgress; 
            set => SetProperty(ref this._maxProgress,value);
        }
        public string ProgressLabel { 
            get => this._progressLabel; 
            set => SetProperty(ref this._progressLabel,value); 
        }
        public string ButtonLabel { 
            get => this._buttonLabel; 
            set => SetProperty(ref this._buttonLabel,value); 
        }

        private void SetProgressMessage() {
            switch (this._installTraveler.InstallStatus) {
                case InstallStatus.InstalledNewVersion:
                    this.ProgressLabel = "Updating....";
                    break;
                case InstallStatus.InstalledUpToDate:
                    break;
                case InstallStatus.NotInstalled:
                    this.ProgressLabel = "Installing....";
                    break;
                case InstallStatus.ServerFilesMissing:
                    break;
            }
        }

        private async Task InstallHandler() {

            this.IsInstalling = true;
            this._installer.InstallLocation = this._installLocation;
            this.MaxProgress = this._installer.CalculateWork();

            var token = this._tokenSource.Token;
            Task task;
            if (!string.IsNullOrEmpty(this._installLocation)) {
                task=this._installer.InstallAsync(token).ContinueWith(this.AfterInstall,TaskContinuationOptions.RunContinuationsAsynchronously);
            } else {
                task = this._installer.InstallAsync(token, this._installLocation).ContinueWith(this.AfterInstall,TaskContinuationOptions.RunContinuationsAsynchronously);
            }
            await Task.WhenAll(task);
        }

        private async Task AfterInstall(Task<bool> data) {
            if (this._tokenSource.IsCancellationRequested) {
                this.ProgressLabel = "Uninstall Progress";
                this.InstallLog = "Removing Files..." + Environment.NewLine;
                this.MaxProgress = this._installer.CalculateWorkUninstall();
                this.ItemCount = 0;
                this._installer.InstallLocation = this._installLocation;
                this._tokenSource = new CancellationTokenSource();
                await this._installer.UnInstall().ContinueWith((data) => { 
                    this.IsInstalling = false;
                    this.DispatcherService.BeginInvoke(() => {
                        var result=this.MessageBox.ShowMessage("Finished Removing Files, Closing Installer","Goodbye",MessageButton.OK,MessageIcon.Exclamation);
                        if (result == MessageResult.OK) {
                            this.CurrentWindowService.Close();
                        } else {
                            this.CurrentWindowService.Close();
                        }
                    });
                });
            } else {
                this.IsInstalling = false;
                this.InstallLog = (data.Result) ? ("Install Complete" + Environment.NewLine) : ("Install Failed" + Environment.NewLine);
            }
        }

        private async Task IncrementProgressHandler(string logLine) {
            await Task.Run(() => {
                this.ItemCount = this.ItemCount + 1;
                string line = DateTime.Now + ": " + logLine + Environment.NewLine;
                this.InstallLog += line;
            });
        }

        private void Cancel() {
            if (this._installing) {
                this._tokenSource.Cancel();
            } else {
                this._eventAggregator.GetEvent<CancelEvent>().Publish();
            }
        }

        private void GoBack() {
            _journal.GoBack();
        }

        private void GoForward() {
            var parameters = new NavigationParameters();
            parameters.Add("Success", true);
            this._regionManager.RequestNavigate("ContentRegion", "FinishedView", parameters);
            this._journal.GoForward();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 
        
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._journal = navigationContext.NavigationService.Journal;
            var installLocation = navigationContext.Parameters["InstallLocation"] as string;
            this._installLocation = installLocation;
        }
    }
}
