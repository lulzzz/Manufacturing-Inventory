using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
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
using ManufacturingInventory.Application.Boundaries.PartDetails;
using System.Text;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartSummaryViewModel : InventoryViewModelBase {

        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartSummaryDispatcher"); } }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PartSummaryMessageBox"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPartSummaryEditUseCase _partSummaryEdit;

        private bool _isNew = false;
        private bool _isEdit = false;
        private bool _canSaveCancel = false;
        private bool _isBubbler = false;

        private ObservableCollection<Warehouse> _warehouses = new ObservableCollection<Warehouse>();
        private ObservableCollection<Organization> _organizations = new ObservableCollection<Organization>();
        private ObservableCollection<Usage> _usageList = new ObservableCollection<Usage>();

        private Part _selectedPart;
        private int _selectedPartId;
        private Organization _selectedOrganization;
        private Warehouse _selectedWarehouse;
        private Usage _selectedUsage;

        private string _name;
        private string _description;
        private bool _holdsBubblers;

        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public PartSummaryViewModel(IPartSummaryEditUseCase partSummaryEdit, IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._partSummaryEdit = partSummaryEdit;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
            this.SaveCommand = new AsyncCommand(this.SaveHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
        }

        public override bool KeepAlive => false;

        public bool IsNew { 
            get => this._isNew;
            set => SetProperty(ref this._isNew, value);
        }

        public bool IsEdit { 
            get => this._isEdit; 
            set => SetProperty(ref this._isEdit,value);
        }

        public bool CanSaveCancel { 
            get => this._canSaveCancel;
            set => SetProperty(ref this._canSaveCancel, value);
        }

        public int SelectedPartId { 
            get => this._selectedPartId;
            set => SetProperty(ref this._selectedPartId, value);
        }

        public Part SelectedPart {
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart, value);
        }

        public ObservableCollection<Warehouse> Warehouses {
            get => this._warehouses;
            set => SetProperty(ref this._warehouses, value);
        }

        public ObservableCollection<Organization> Organizations {
            get => this._organizations;
            set => SetProperty(ref this._organizations, value);
        }

        public ObservableCollection<Usage> UsageList {
            get => this._usageList;
            set => SetProperty(ref this._usageList, value);
        }

        public Organization SelectedOrganization {
            get => this._selectedOrganization;
            set => SetProperty(ref this._selectedOrganization, value, "SelectedOrganization");
        }

        public Warehouse SelectedWarehouse {
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value, "SelectedWarehouse");
        }

        public Usage SelectedUsage {
            get => this._selectedUsage;
            set => SetProperty(ref this._selectedUsage, value, "SelectedUsage");
        }

        public bool IsBubbler {
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public string Name {
            get => this._name;
            set => SetProperty(ref this._name, value);
        }

        public string Description { 
            get => this._description;
            set => SetProperty(ref this._description, value);
        }

        public bool HoldsBubblers { 
            get => this._holdsBubblers;
            set => SetProperty(ref this._holdsBubblers, value);
        }

        private async Task SaveHandler() {
            int id = (this.IsNew) ? 0 : this.SelectedPartId;
            int warehouseId = (this.SelectedWarehouse != null) ? this.SelectedWarehouse.Id : 0;
            int orgId = (this.SelectedOrganization != null) ? this.SelectedOrganization.Id : 0;
            int useageId = (this.SelectedUsage != null) ? this.SelectedUsage.Id : 0;


            PartSummaryEditInput input = new PartSummaryEditInput(id, this.Name, this.Description,this.IsNew ,this.HoldsBubblers,warehouseId,orgId,useageId);

            var output = await this._partSummaryEdit.Execute(input);
            if (output.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(output.Message,"Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<PartEditDoneEvent>().Publish(output.Part.Id);
                    this.CanSaveCancel = false;
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    var responce=this.MessageBoxService.ShowMessage("Save failed"+Environment.NewLine+"Check Input and Try Again?", "Error",MessageButton.YesNo, MessageIcon.Error);
                    if (responce == MessageResult.No) {
                        this._eventAggregator.GetEvent<PartEditCancelEvent>().Publish();
                        this.CanSaveCancel = false;
                    }
                });
            }
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this._eventAggregator.GetEvent<PartEditCancelEvent>().Publish();
                    this.CanSaveCancel = false;
                });
            });
        }


        private async Task LoadAsync() {
            var warehouses = await this._partSummaryEdit.GetWarehouses();
            var categories = await this._partSummaryEdit.GetCategories();
            this.SelectedPart = await this._partSummaryEdit.GetPart(this.SelectedPartId);
                   
            this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
            this.UsageList = new ObservableCollection<Usage>(categories.OfType<Usage>());
            this.Organizations = new ObservableCollection<Organization>(categories.OfType<Organization>());

            if (this._selectedPart.Warehouse != null) {
                this.SelectedWarehouse = this.Warehouses.FirstOrDefault(e => e.Id == this._selectedPart.WarehouseId);
            }

            if (this._selectedPart.Usage != null) {
                this.SelectedUsage = this.UsageList.FirstOrDefault(e => e.Id == this._selectedPart.UsageId);
            }


            if (this._selectedPart.Organization != null) {
                this.SelectedOrganization = this.Organizations.FirstOrDefault(e => e.Id == this._selectedPart.Id);
            }
        }
    }
}
