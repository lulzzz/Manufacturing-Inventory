using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.DistributorManagment.Internal;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ManufacturingInventory.Domain.DTOs;
using System;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using System.Windows;
using System.Collections.Generic;

namespace ManufacturingInventory.DistributorManagment.ViewModels {
    public class DistributorDetailsViewModel : InventoryViewModelNavigationBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DistributorDetailsDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("DistributorDetailsMessageBox"); }
        protected IDialogService NewContactDialog { get => ServiceContainer.GetService<IDialogService>("NewContactDialog"); }

        private IDistributorEditUseCase _distributorEdit;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private int _distributorId;
        private bool _isEdit;
        private bool _canEdit;
        private ContactDataTraveler _contactDataTraveler;
        private AttachmentDataTraveler _attachmentTraveler;
        private ObservableCollection<ContactDTO> _contacts;
        private ContactDTO _selectedContact;
        private ObservableCollection<Price> _prices;
        private Distributor _selectedDistributor;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand AddNewContactCommand { get; private set; }
        public AsyncCommand DeleteContactCommand { get; private set; }
        public AsyncCommand EditContactCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

        public DistributorDetailsViewModel(IDistributorEditUseCase distributorEditUseCase,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._distributorEdit = distributorEditUseCase;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
        }

        public override bool KeepAlive => false;

        public AttachmentDataTraveler AttachmentDataTraveler { 
            get => this._attachmentTraveler; 
            set => SetProperty(ref this._attachmentTraveler,value); 
        }
        
        public ObservableCollection<ContactDTO> Contacts { 
            get => this._contacts;
            set => SetProperty(ref this._contacts, value);
        }

        public ContactDTO SelectedContact { 
            get => this._selectedContact;
            set => SetProperty(ref this._selectedContact, value);
        }

        public ObservableCollection<Price> Prices { 
            get => this._prices;
            set => SetProperty(ref this._prices, value);
        }

        public bool CanEdit { 
            get => this._canEdit;
            set => SetProperty(ref this._canEdit, value);
        }

        public Distributor SelectedDistributor { 
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }

        public ContactDataTraveler ContactDataTraveler {
            get => this._contactDataTraveler; 
            set =>SetProperty(ref this._contactDataTraveler,value);
        }

        private Task SaveHandler() {
            return Task.CompletedTask;
        }

        private Task CancelHandler() {
            return Task.CompletedTask;
        }

        private async Task Load() {
            var distributor = await this._distributorEdit.GetDistributor(this._distributorId);
            var contacts = await this._distributorEdit.GetContacts(this._distributorId);
            var prices = await this._distributorEdit.GetPrices(this._distributorId);
            this.SelectedDistributor = distributor;
            this.DispatcherService.BeginInvoke(() => {

                this.Prices = new ObservableCollection<Price>();
                this.Contacts = new ObservableCollection<ContactDTO>(contacts);
                
                this.CanEdit = this._isEdit;

            });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._distributorId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.SelectedDistributorId]);
            this._isEdit = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
            this.AttachmentDataTraveler = new AttachmentDataTraveler(GetAttachmentBy.DISTRIBUTOR, this._distributorId);
            this.ContactDataTraveler = new ContactDataTraveler(this._distributorId, !this._isEdit);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }
    }
}
