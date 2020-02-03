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
        private AttachmentDataTraveler _attachmentTraveler;
        private Price _price;
        private int _priceId;
        private double _unitCost;
        private DateTime _timeStamp;
        private DateTime? _validFrom;
        private DateTime? _validUntil;
        private int _minOrder;
        private double _leadTime;

        private bool _isEdit;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

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

        public ObservableCollection<Distributor> Distributors { 
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public Distributor SelectedDistributor { 
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }
        public double UnitCost { 
            get => this._unitCost;
            set => SetProperty(ref this._unitCost, value);
        }

        public DateTime TimeStamp { 
            get => this._timeStamp;
            set => SetProperty(ref this._timeStamp, value);
        }

        public DateTime? ValidFrom { 
            get => this._validFrom;
            set => SetProperty(ref this._validFrom, value);
        }

        public DateTime? ValidUntil { 
            get => this._validUntil;
            set => SetProperty(ref this._validUntil, value);
        }

        public int MinOrder { 
            get => this._minOrder;
            set => SetProperty(ref this._leadTime, value);
        }

        public double LeadTime { 
            get => this._leadTime;
            set => SetProperty(ref this._leadTime, value);
        }

        public AttachmentDataTraveler AttachmentDataTraveler { 
            get => this._attachmentTraveler;
            set => SetProperty(ref this._attachmentTraveler, value);
        }

        public bool IsEdit {
            get => this._isEdit;
            set => SetProperty(ref this._isEdit, value);
        }


        private async Task SaveHandler() {
            //PriceEditInput input=new PriceEditInput()
        }

        private async Task CancelHandler() {

        }

        private async Task InitializeHandler() {
            if (!this._isInitialized) {
                var distributors =await this._priceEdit.GetDistributors();
                var price= await this._priceEdit.GetPrice(this.PriceId);

                this.AttachmentDataTraveler = new AttachmentDataTraveler(Domain.Enums.GetAttachmentBy.PRICE, this.PriceId);
                this.UnitCost = price.UnitCost;
                this.LeadTime = price.LeadTime;
                this.ValidFrom = price.ValidFrom;
                this.ValidUntil = price.ValidUntil;
                this.TimeStamp = price.TimeStamp;
                this.MinOrder = price.MinOrder;
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
