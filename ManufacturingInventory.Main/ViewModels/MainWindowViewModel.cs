using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using System;
using Serilog;
using System.Printing;
using System.Threading.Tasks;
using Castle.Core.Logging;

namespace ManufacturingInventory.ManufacturingApplication.ViewModels {
    public class MainWindowViewModel : InventoryViewModelBase {
        private IRegionManager _regionManager;
        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("Notice"); } }
        //protected INotificationService UpdateNotificationService { get { return ServiceContainer.GetService<INotificationService>("UpdateNotificationService"); } }
        //private SparkleUpdater _sparkle;
        private Serilog.ILogger _logger;

        public Prism.Commands.DelegateCommand<string> LoadModuleCommand { get; private set; }
        public Prism.Commands.DelegateCommand LoadedCommand { get; private set; }

        public MainWindowViewModel(IRegionManager regionManager,Serilog.ILogger logger) {
            this._regionManager = regionManager;
            this.LoadModuleCommand = new PrismCommands.DelegateCommand<string>(this.LoadModuleHandler);
            this.LoadedCommand = new Prism.Commands.DelegateCommand(this.LoadedHandler);
            this._logger = logger;
        }

        private string _title = "Manufacturing Inventory";
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public override bool KeepAlive {
            get => true;
        }


        private void LoadedHandler() {
            if (DXSplashScreen.IsActive)
                DXSplashScreen.Close();
        }

        private void LoadModuleHandler(string navigationPath) {
            if (!string.IsNullOrEmpty(navigationPath)) {
                this._regionManager.RequestNavigate(Regions.MainWindowRegion, navigationPath);
            }
        }
    }
}
