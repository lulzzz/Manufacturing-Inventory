using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Common.Application;
using Prism.Events;
using ManufacturingInventory.Application.Boundaries.ContactTableDetailEdit;
using System.Threading.Tasks;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Domain.DTOs;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using System.Linq;
using System.Windows;
using ManufacturingInventory.Application.Boundaries;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class ContactTableDetailViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("ContactDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("ContactMessageBox"); }
        protected IDialogService NewContactDialog { get => ServiceContainer.GetService<IDialogService>("ContactDialog"); }

        private IEventAggregator _eventAggregator;
        private IContactTableDetailEditUseCase _contactUseCase;

        private bool _isInitialized=false;
        private bool _editInProgress;
        private int _distributorId;
        private NewContactViewModel _newContactViewModel;
        private ContactDataTraveler _contactDataTraveler;
        private ObservableCollection<ContactDTO> _contacts;
        private ContactDTO _selectedContact;
        private bool _showTableLoading;
        private bool _canEdit;

        public AsyncCommand AddContactCommand { get; private set; }
        public AsyncCommand EditContactCommand { get; private set;}
        public AsyncCommand DeleteContactCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public ContactTableDetailViewModel(IContactTableDetailEditUseCase contactUseCase,IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
            this._contactUseCase = contactUseCase;
            this.AddContactCommand = new AsyncCommand(this.AddNewContactHandler, () => (!this._editInProgress && this.CanEdit));
            this.EditContactCommand = new AsyncCommand(this.EditContactHandler, () => (!this._editInProgress && this.CanEdit));
            this.DeleteContactCommand = new AsyncCommand(this.DeleteContactHandler,()=>(!this._editInProgress && this.CanEdit));
            this.InitializeCommand = new AsyncCommand(this.Load);
        }

        public override bool KeepAlive => false;

        public bool CanEdit { 
            get => this._canEdit;
            set => SetProperty(ref this._canEdit, value);
        }

        public ContactDataTraveler ContactDataTraveler { 
            get => this._contactDataTraveler;
            set => SetProperty(ref this._contactDataTraveler, value);
        }

        public ObservableCollection<ContactDTO> Contacts { 
            get => this._contacts; 
            set=>SetProperty(ref this._contacts,value);
        }

        public ContactDTO SelectedContact { 
            get => this._selectedContact;
            set => SetProperty(ref this._selectedContact, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value); 
        }

        public int DistributorId { 
            get => this._distributorId; 
            set => this._distributorId = value; 
        }

        private async Task AddNewContactHandler() {
            this._editInProgress = true;
            if (this.ShowNewContactDialog()) {
                ContactTableDetailEditInput input = new ContactTableDetailEditInput(EditAction.Add, this._newContactViewModel.Contact, this._distributorId);
                var response = await this._contactUseCase.Execute(input);
                await this.ShowActionResponseAndReload(response);
            }
            this._editInProgress = false;
        }

        private async Task DeleteContactHandler() {
            if (this.SelectedContact != null) {
                this._editInProgress = true;
                ContactTableDetailEditInput input = new ContactTableDetailEditInput(EditAction.Delete, this.SelectedContact, this._distributorId);
                var response = await this._contactUseCase.Execute(input);
                await this.ShowActionResponseAndReload(response);
                this._editInProgress = false;
            }
        }

        private async Task EditContactHandler() {
            this._editInProgress = true;
            if (this.ShowEditContactDialog()) {
                ContactTableDetailEditInput input = new ContactTableDetailEditInput(EditAction.Update, this._newContactViewModel.Contact, this._distributorId);
                var response = await this._contactUseCase.Execute(input);
                await this.ShowActionResponseAndReload(response);
            }
            this._editInProgress = false;
        }

        private bool ShowNewContactDialog() {
            if (this._newContactViewModel == null) {
                this._newContactViewModel = new NewContactViewModel();
            }

            UICommand saveCommand = new UICommand() {
                Caption = "Add Contact",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };

            UICommand result = this.NewContactDialog.ShowDialog(
                dialogCommands: new List<UICommand>() { saveCommand, cancelCommand },
                title: "New Distributor",
                viewModel: this._newContactViewModel);
            return result == saveCommand;
        }

        private bool ShowEditContactDialog() {
            if (this._newContactViewModel == null) {
                this._newContactViewModel = new NewContactViewModel();
            }
            this._newContactViewModel.Contact = this.SelectedContact;

            UICommand saveCommand = new UICommand() {
                Caption = "Save Changes",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };

            UICommand result = this.NewContactDialog.ShowDialog(
                dialogCommands: new List<UICommand>() { saveCommand, cancelCommand },
                title: "Edit Contact",
                viewModel: this._newContactViewModel);
            return result == saveCommand;
        }

        private async Task ShowActionResponseAndReload(ContactTableDetailEditOutput response) {
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                });
                await this.Reload(response.Contact.Id);

            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
                await this.Reload();
            }
        }

        private async Task Reload(int? contactId=null) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            var contacts = await this._contactUseCase.GetContacts(this._distributorId);
            this.DispatcherService.BeginInvoke(() => {
                this.Contacts = new ObservableCollection<ContactDTO>(contacts);
                if (contactId.HasValue) {
                    this.SelectedContact = this.Contacts.FirstOrDefault(e => e.Id == contactId);
                }
                this.ShowTableLoading = false;
            });
        }

        private async Task Load() {
            if (!this._isInitialized) {
                
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var contacts = await this._contactUseCase.GetContacts(this._distributorId);
                this.DispatcherService.BeginInvoke(() => {
                    this.Contacts = new ObservableCollection<ContactDTO>(contacts);
                    this.ShowTableLoading = false;
                });
                this._isInitialized = true;
            }
        }
    }
}
