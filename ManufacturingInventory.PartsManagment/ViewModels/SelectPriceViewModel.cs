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
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using System.IO;
using System.Collections.Generic;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class SelectPriceViewModel : InventoryViewModelNavigationBase {
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PriceSelectTableMessageBoxService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PriceSelectTableDispatcher"); }


        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPriceEditUseCase _priceEdit;

        private int? _priceId;
        private int _partId;
        private int _instanceId;
        private bool _isInitialized = false;
        private bool _showTableLoading = false;
        private ObservableCollection<Price> _prices;
        private Price _selectedPrice;


        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public SelectPriceViewModel(IPriceEditUseCase priceEdit, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._priceEdit = priceEdit;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.SaveCommand = new AsyncCommand(this.SaveHandler,this.CanSave);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Price> Prices { 
            get => this._prices;
            set => SetProperty(ref this._prices, value);
        }

        public Price SelectedPrice { 
            get => this._selectedPrice;
            set => SetProperty(ref this._selectedPrice, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        private async Task SaveHandler() {
            PriceEditInput input = new PriceEditInput(this.SelectedPrice.Id, this._instanceId, this._partId, PriceEditAction.ReplaceWithExisiting);
            var response = await this._priceEdit.Execute(input);
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<PriceEditDoneEvent>().Publish();
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private Task CancelHandler() {
            return Task.Factory.StartNew(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var response = this.MessageBoxService.ShowMessage("Are you sure you want to cancel?", "Cancel?", MessageButton.YesNo, MessageIcon.Question);
                    if (response == MessageResult.Yes) {
                        this._eventAggregator.GetEvent<PriceEditDoneEvent>().Publish();
                    }
                });
            }); 
        }

        private bool CanSave() {
            return this.SelectedPrice != null;
        }

        private async Task LoadAsync() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            var prices = await this._priceEdit.GetPartPrices(this._partId);
            this.DispatcherService.BeginInvoke(() => {
                this.Prices=new ObservableCollection<Price>(prices);
                if (this._priceId.HasValue) {
                    this.SelectedPrice = this.Prices.FirstOrDefault(e => e.Id == this._priceId.Value);
                }
                this.ShowTableLoading = false;
            });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._priceId = navigationContext.Parameters[ParameterKeys.PriceId] as int?;
            this._instanceId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.InstanceId]);
            this._partId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }
    }
}
