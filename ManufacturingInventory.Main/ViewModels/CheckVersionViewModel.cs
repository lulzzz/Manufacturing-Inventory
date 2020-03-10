using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.ManufacturingApplication.Services;

namespace ManufacturingInventory.ManufacturingApplication.ViewModels {
    public class CheckVersionViewModel : InventoryViewModelBase {


        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("CheckVersionDispatcher"); } }

        public event EventHandler CheckCompleted;

        private CheckVersionResponse _checkVersionResponse;
        bool _isWaitIndicatorVisible;
        string _waitIndicatorText;
        private string _message;
        

        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand UpdateLaterCommand { get; private set; }
        public DelegateCommand InitializeCommand { get; private set; }

        public CheckVersionViewModel(CheckVersionResponse checkVersionResponse) {
            this.UpdateCommand = new DelegateCommand(this.UpdateHandler, !this.IsWaitIndicatorVisible);
            this.UpdateLaterCommand = new DelegateCommand(this.UpdateLaterHandler, !this.IsWaitIndicatorVisible);
            this.InitializeCommand = new DelegateCommand(this.CheckVersion);
            this._checkVersionResponse = checkVersionResponse;
            this.WaitIndicatorText = "Checking Version, Please Wait....";

        }

        private void CheckVersion() {
            this.Message = "Checking Version";
            this.IsWaitIndicatorVisible = true;
            StringBuilder builder = new StringBuilder();
            if (this._checkVersionResponse.NewVersionAvailable) {
                builder.AppendLine("New Version is Available!");
                builder.AppendFormat("Current Version: {0}", this._checkVersionResponse.CurrentVersion).AppendLine();
                builder.AppendFormat("New Version: {0}", this._checkVersionResponse.NewVersion).AppendLine();
            } else {
                builder.AppendLine("You Are On Most Update Version");
                builder.AppendFormat("Current Version: {0}", this._checkVersionResponse.CurrentVersion).AppendLine();
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

        public override bool KeepAlive => false;

        public bool Update { get; set; }

        private void UpdateHandler() {
            this.Update = true;
            this.CheckCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateLaterHandler() {
            this.Update = false;
            this.CheckCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
