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
        //protected IDialogService NewContactDialog { get => ServiceContainer.GetService<IDialogService>("NewContactDialog"); }

        private IDistributorEditUseCase _distributorEdit;
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        private int _distributorId;
        private bool _isEdit;
        private bool _canEdit;
        private string _name;
        private string _description;
        private ContactDataTraveler _contactDataTraveler;
        private AttachmentDataTraveler _attachmentTraveler;
        private ObservableCollection<ContactDTO> _contacts;
        //private ContactDTO _selectedContact;
        private ObservableCollection<Price> _prices;
        private Distributor _selectedDistributor;
        private bool _showPriceTableLoading;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

        public DistributorDetailsViewModel(IDistributorEditUseCase distributorEditUseCase,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._distributorEdit = distributorEditUseCase;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.Load);
            this.SaveCommand = new AsyncCommand(this.SaveHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler); 
        }

        public override bool KeepAlive => false;

        public AttachmentDataTraveler AttachmentDataTraveler { 
            get => this._attachmentTraveler; 
            set => SetProperty(ref this._attachmentTraveler,value); 
        }
        
        //public ObservableCollection<ContactDTO> Contacts { 
        //    get => this._contacts;
        //    set => SetProperty(ref this._contacts, value);
        //}

        //public ContactDTO SelectedContact { 
        //    get => this._selectedContact;
        //    set => SetProperty(ref this._selectedContact, value);
        //}

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
        public string Name { 
            get => this._name; 
            set => SetProperty(ref this._name, value); 
        }
        
        public string Description { 
            get => this._description;
            set => SetProperty(ref this._description, value);
        }
        public bool ShowPriceTableLoading { 
            get => this._showPriceTableLoading;
            set => SetProperty(ref this._showPriceTableLoading, value);
        }

        private async Task SaveHandler() {
            DistributorEditInput input = new DistributorEditInput(this._distributorId, this.Name, this.Description, Application.Boundaries.EditAction.Update);
            var response = await this._distributorEdit.Execute(input);
            await this.ShowActionResponseAndReload(response);
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                this._eventAggregator.GetEvent<DistributorEditCancelEvent>().Publish(this._distributorId);
            });
        }

        private async Task ShowActionResponseAndReload(DistributorEditOutput response) {
            await Task.Run(() => {
                if (response.Success) {
                    this.DispatcherService.BeginInvoke(() => {
                        var resp=this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information,MessageResult.OK);
                        if (resp == MessageResult.OK) {
                            this._eventAggregator.GetEvent<DistributorEditDoneEvent>().Publish(response.Distributor.Id);
                        } else {
                            this._eventAggregator.GetEvent<DistributorEditDoneEvent>().Publish(response.Distributor.Id);
                        }

                    });
                } else {
                    this.DispatcherService.BeginInvoke(() => {
                        var resp = this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error, MessageResult.OK);
                        if (resp == MessageResult.OK) {
                            this._eventAggregator.GetEvent<DistributorEditDoneEvent>().Publish(response.Distributor.Id);
                        } else {
                            this._eventAggregator.GetEvent<DistributorEditDoneEvent>().Publish(response.Distributor.Id);
                        }
                        this._eventAggregator.GetEvent<DistributorEditCancelEvent>().Publish(response.Distributor.Id);
                    });
                }
            });
        }

        private async Task Load() {
            this.DispatcherService.BeginInvoke(() =>this.ShowPriceTableLoading=true);
            var distributor = await this._distributorEdit.GetDistributor(this._distributorId);
            //var contacts = await this._distributorEdit.GetContacts(this._distributorId);
            var prices = await this._distributorEdit.GetPrices(this._distributorId);
            this.SelectedDistributor = distributor;
            this.DispatcherService.BeginInvoke(() => {
                this.Name = distributor.Name;
                this.Description = distributor.Description;
                this.Prices = new ObservableCollection<Price>(prices);
               // this.Contacts = new ObservableCollection<ContactDTO>(contacts);          
                this.CanEdit = this._isEdit;
                this.ShowPriceTableLoading =false;
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
