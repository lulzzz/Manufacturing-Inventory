using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Mvvm;
using ManufacturingInventory.ManufacturingApplication.Services;

namespace ManufacturingInventory.ManufacturingApplication.ViewModels {
    public class CheckVersionViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices {
        public DevExpress.Mvvm.IServiceContainer _serviceContainer = null;
        protected DevExpress.Mvvm.IServiceContainer ServiceContainer {
            get {
                if (this._serviceContainer == null) {
                    this._serviceContainer = new DevExpress.Mvvm.ServiceContainer(this);
                }
                return this._serviceContainer;
            }
        }

        DevExpress.Mvvm.IServiceContainer DevExpress.Mvvm.ISupportServices.ServiceContainer { get { return ServiceContainer; } }

        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("CheckVersionDispatcher"); } }

        public event EventHandler CheckCompleted;

        private CheckVersion _checkVersion;
        bool _isWaitIndicatorVisible;
        string _waitIndicatorText;
        private string _message;

        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand UpdateLaterCommand { get; private set; }
        public DelegateCommand InitializeCommand { get; private set; }

        public CheckVersionViewModel() {
            this.UpdateCommand = new DelegateCommand(this.UpdateHandler, !this.IsWaitIndicatorVisible);
            this.UpdateLaterCommand = new DelegateCommand(this.UpdateLaterHandler, !this.IsWaitIndicatorVisible);
            this.InitializeCommand = new DelegateCommand(this.CheckVersion);
            this._checkVersion = new CheckVersion();
            this.WaitIndicatorText = "Checking Version, Please Wait....";

        }

        private void CheckVersion() {
            this.Message = "Checking Version";
            this.IsWaitIndicatorVisible = true;
            var responce=this._checkVersion.Check();
            StringBuilder builder = new StringBuilder();
            if (responce.NewVersionAvailable) {
                builder.AppendLine("New Version is Available!");
                builder.AppendFormat("Current Version: {0}",responce.CurrentVersion).AppendLine();
                builder.AppendFormat("New Version: {0}", responce.NewVersion).AppendLine();
            } else {
                builder.AppendLine("You Are On Most Update Version");
                builder.AppendFormat("Current Version: {0}", responce.CurrentVersion).AppendLine();
            }
            this.Message = builder.ToString();
            this.IsWaitIndicatorVisible = false;
        }

        public bool IsWaitIndicatorVisible {
            get => this._isWaitIndicatorVisible;
            set => SetProperty(ref this._isWaitIndicatorVisible, value);
        }

        public string WaitIndicatorText {
            get => this._waitIndicatorText;
            set => SetProperty(ref this._waitIndicatorText, value);
        }

        public string Message {
            get => this._message;
            set => SetProperty(ref this._message, value);
        }

        private void UpdateHandler() {
            this.CheckCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateLaterHandler() {
            this.CheckCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
