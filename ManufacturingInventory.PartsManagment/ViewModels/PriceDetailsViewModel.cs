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

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PriceDetailsViewModel : InventoryViewModelBase {

        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PriceDetailsMessageService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PriceDetailDispatcher"); } 


        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPriceEditUseCase _priceEdit;
        private ObservableCollection<Distributor> _distributors;
        private Distributor _selectedDistributor;
        private Price _price;
        private int _priceId;
        private bool _isEdit;
        private bool _isInitialized = false;

        public PriceDetailsViewModel(IPriceEditUseCase priceEditUseCase,IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._priceEdit = priceEditUseCase;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
        }

        public override bool KeepAlive => false;

        public Price Price { 
            get => this._price;
            set => SetProperty(ref this._price, value);
        }

        public int PriceId { 
            get => this._priceId;
            set => this._priceId=value;
        }

        public bool IsEdit { 
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, value);
        }

        public ObservableCollection<Distributor> Distributors { 
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public Distributor SelectedDistributor { 
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }

        private async Task InitializeHandler() {
            if (!this._isInitialized) {
                var distributors =await this._priceEdit.GetDistributors();
                var price= await this._priceEdit.GetPrice(this.PriceId);
                this.DispatcherService.BeginInvoke(() => {
                    this.Price = price;
                    this.Distributors = new ObservableCollection<Distributor>(distributors);
                    this.SelectedDistributor = this.Distributors.FirstOrDefault(e => e.Id == this.Price.DistributorId);
                });
                this._isInitialized = true;
            }
        }
    }
}
