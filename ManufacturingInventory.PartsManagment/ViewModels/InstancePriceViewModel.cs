using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Domain.Enums;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class InstancePriceViewModel : InventoryViewModelBase {

        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("InstancePriceMessageService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("InstancePriceDispatcher"); }

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private bool _hasPrice;
        private bool _isNew;
        private bool _editInProgress=false;

        public AsyncCommand NewPriceCommand { get; private set; }
        public AsyncCommand SelectPriceCommand { get; private set; }
        public AsyncCommand EditPriceCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public InstancePriceViewModel(IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.NewPriceCommand = new AsyncCommand(this.NewPriceHandler, this.CanExecute);
            this.SelectPriceCommand = new AsyncCommand(this.SelectPriceHandler, this.CanExecute);
            this.EditPriceCommand = new AsyncCommand(this.EditPriceHandler,this.CanExecute);
            this.InitializeCommand = new AsyncCommand(this.LoadHandler);
        }

        public override bool KeepAlive => false;
        public int? PriceId { get; set; }
        public int PartId { get; set; }
        public int? InstanceId { get; set; }

        public bool IsNew { 
            get=>this._isNew; 
            set=>SetProperty(ref this._isNew,value);
        }

        public bool HasPrice { 
            get => this._hasPrice;
            set => SetProperty(ref this._hasPrice, value);
        }

        private Task NewPriceHandler() {
            this._editInProgress = true;
            this.CleanupRegions();
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.PriceId);
            param.Add(ParameterKeys.InstanceId, this.InstanceId);
            param.Add(ParameterKeys.PartId, this.PartId);
            param.Add(ParameterKeys.IsEdit, false);
            param.Add(ParameterKeys.IsNew, true);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.PriceDetailsView, param);
            return Task.CompletedTask;
        }

        private Task SelectPriceHandler() {
            this._editInProgress = true;
            this.CleanupRegions();
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.PriceId);
            param.Add(ParameterKeys.InstanceId, this.InstanceId);
            param.Add(ParameterKeys.PartId, this.PartId);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.SelectPriceView, param);
            return Task.CompletedTask;
        }

        private Task EditPriceHandler() {
            this._editInProgress = true;
            this.CleanupRegions();
            NavigationParameters param = new NavigationParameters();
            param.Add(ParameterKeys.PriceId, this.PriceId);
            param.Add(ParameterKeys.InstanceId, this.InstanceId);
            param.Add(ParameterKeys.PartId, this.PartId);
            param.Add(ParameterKeys.IsEdit, true);
            param.Add(ParameterKeys.IsNew, false);
            this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.PriceDetailsView, param);
            return Task.CompletedTask;
        }

        private async Task InstanceCreatedHandler(int instanceId) {
            this.IsNew = false;
            this.InstanceId = instanceId;
            await this.LoadHandler();
        }

        private bool CanExecute() {
            return !this._editInProgress && !this.IsNew;
        }

        private Task LoadHandler() {
            if (this.PriceId.HasValue) {
                this.HasPrice = true;
                this.CleanupRegions();
                NavigationParameters param = new NavigationParameters();
                param.Add(ParameterKeys.PriceId, this.PriceId);
                param.Add(ParameterKeys.InstanceId, this.InstanceId);
                param.Add(ParameterKeys.PartId, this.PartId);
                param.Add(ParameterKeys.IsEdit, false);
                param.Add(ParameterKeys.IsNew, false);
                this._regionManager.RequestNavigate(LocalRegions.InstancePriceEditDetailsRegion, ModuleViews.PriceDetailsView, param);
            } else {
                this.HasPrice = false;
            }
            return Task.CompletedTask;
        }

        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.InstancePriceEditDetailsRegion].RemoveAll();
        }
    }
}
