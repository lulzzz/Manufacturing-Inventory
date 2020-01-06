using Prism.Commands;
using Prism.Mvvm;
using DevExpress.Xpf.Core;
using ManufacturingInventory.Common.Application;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using ManufacturingInventory.Common.Model;
using System.Windows;
using ManufacturingInventory.Common.Model.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartSummaryViewModel : InventoryViewModelBase {

        protected IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartSummaryDispatcher"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        private bool _isNewPart = false;
        private bool _isEdit = false;
        private bool _isBubbler;

        private ObservableCollection<Warehouse> _warehouses = new ObservableCollection<Warehouse>();
        private ObservableCollection<Organization> _organizations = new ObservableCollection<Organization>();
        private ObservableCollection<Usage> _usageList = new ObservableCollection<Usage>();

        private Part _selectedPart;
        private Organization _selectedOrganization;
        private Warehouse _selectedWarehouse;
        private Usage _selectedUsage;
        private int _selectedWarehouseIndex;
        private int _selectedUsageIndex;
        private int _selectedOrganizationIndex;
        public AsyncCommand InitializeCommand { get; private set; }

        public PartSummaryViewModel(ManufacturingContext context,IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._context = context;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
        }

        public override bool KeepAlive => false;

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

        public int SelectedWarehouseIndex { 
            get => this._selectedWarehouseIndex;
            set => SetProperty(ref this._selectedWarehouseIndex, value);
        }

        public int SelectedUsageIndex { 
            get => this._selectedUsageIndex;
            set => SetProperty(ref this._selectedUsageIndex, value);
        }

        public int SelectedOrganizationIndex { 
            get => this._selectedOrganizationIndex;
            set => SetProperty(ref this._selectedOrganizationIndex, value);
        }

        private async Task LoadAsync() {
            var warehouses = await this._context.Locations.AsNoTracking().OfType<Warehouse>().ToListAsync();
            this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
            if (this._selectedPart.Warehouse != null) {               
                this.SelectedWarehouse = this.Warehouses.FirstOrDefault(e => e.Id == this._selectedPart.WarehouseId);
                this.SelectedWarehouseIndex = this.Warehouses.IndexOf(this.SelectedWarehouse);
            }

            var usageList = await this._context.Categories.AsNoTracking().OfType<Usage>().ToListAsync();
            this.UsageList =new ObservableCollection<Usage>(usageList);
            if (this._selectedPart.Usage != null) {
                this.SelectedUsage = this.UsageList.FirstOrDefault(e => e.Id == this._selectedPart.UsageId);
            }

            var orgs = await this._context.Categories.AsNoTracking().OfType<Organization>().ToListAsync();
            this.Organizations = new ObservableCollection<Organization>(orgs);
            if (this._selectedPart.Organization != null) {

                this.SelectedOrganization = this.Organizations.FirstOrDefault(e => e.Id == this._selectedPart.Id);
            }
        }
    }
}
