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
    public class PartsDetailViewModel:InventoryViewModelNavigationBase {

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("PartDetailsNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("PartDetailsDispatcher"); } }
        //public IDialogService FileNameDialog { get { return ServiceContainer.GetService<IDialogService>("FileNameDialog"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }
        public ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>("SaveFileDialog"); } }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        private int _selectedTabIndex = 0;
        private Visibility _visibility;
        private bool _isNewPart = false;
        private bool _isEdit = false;
        private bool _isNotBubbler;
        private bool _isBubbler;


        private ObservableCollection<PartInstance> _partInstances = new ObservableCollection<PartInstance>();
        private ObservableCollection<Warehouse> _warehouses = new ObservableCollection<Warehouse>();
        private ObservableCollection<Organization> _organizations = new ObservableCollection<Organization>();
        private ObservableCollection<Attachment> _attachments = new ObservableCollection<Attachment>();
        private ObservableCollection<Transaction> _transaction = new ObservableCollection<Transaction>();
        private ObservableCollection<Usage> _usageList = new ObservableCollection<Usage>();

        private Part _selectedPart;
        private Organization _selectedOrganization;
        private Warehouse _selectedWarehouse;
        private Attachment _selectedAttachment;
        private Transaction _selectedTransaction;
        private PartInstance _selectedPartInstance;
        private Usage _selectedUsage;


        public AsyncCommand UndoTransactionCommand { get; set; }
        public AsyncCommand ViewInstanceDetailsCommand { get; private set; }
        public AsyncCommand LoadCommand { get; private set; }

        public PrismCommands.DelegateCommand EditInstanceCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewInstanceCommand { get; private set; }

        public PartsDetailViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ManufacturingContext context) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._eventAggregator.GetEvent<LoadPartDetailsEvent>().Subscribe(async ()=>await this.LoadAsync(),ThreadOption.BackgroundThread);
            this.SelectedTabIndex = 0;
        }

        public override bool KeepAlive => false;

        public Part SelectedPart {
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart, value);
        }

        public ObservableCollection<PartInstance> PartInstances { 
            get => this._partInstances; 
            set => this._partInstances = value; 
        }

        public Visibility Visibility {
            get => this._visibility;
            set => SetProperty(ref this._visibility, value);
        }
        public ObservableCollection<Warehouse> Warehouses { 
            get => this._warehouses; 
            set => SetProperty(ref this._warehouses,value);
        }

        public ObservableCollection<Organization> Organizations { 
            get => this._organizations;
            set => SetProperty(ref this._organizations, value);
        }

        public ObservableCollection<Attachment> Attachments { 
            get => this._attachments;
            set => SetProperty(ref this._attachments, value);
        }

        public ObservableCollection<Transaction> Transactions { 
            get => this._transaction;
            set => SetProperty(ref this._transaction, value);
        }

        public ObservableCollection<Usage> UsageList {
            get => this._usageList;
            set => SetProperty(ref this._usageList, value);
        }

        public Organization SelectedOrganization { 
            get => this._selectedOrganization;
            set => SetProperty(ref this._selectedOrganization, value);
        }

        public Warehouse SelectedWarehouse { 
            get => this._selectedWarehouse;
            set => SetProperty(ref this._selectedWarehouse, value);
        }

        public Attachment SelectedAttachment { 
            get => this._selectedAttachment;
            set => SetProperty(ref this._selectedAttachment, value);
        }

        public Transaction SelectedTransaction { 
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value);
        }

        public PartInstance SelectedPartInstance { 
            get => this._selectedPartInstance;
            set => SetProperty(ref this._selectedPartInstance, value);
        }

        public int SelectedTabIndex {
            get => this._selectedTabIndex;
            set => SetProperty(ref this._selectedTabIndex, value);
        }

        public Usage SelectedUsage { 
            get => this._selectedUsage;
            set => SetProperty(ref this._selectedUsage, value);
        }

        public bool IsNotBubbler { 
            get => this._isNotBubbler;
            set => SetProperty(ref this._isNotBubbler, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        private async Task LoadAsync() {
            var instances = await this._context.PartInstances.AsNoTracking()
                .Include(e => e.Attachments)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Condition)
                .Include(e => e.Price)
                .Include(e => e.PartType)
                .Include(e=>e.BubblerParameter)
                .Include(e=>e.Transactions)
                    .ThenInclude(e=>e.Location)
                .Include(e=>e.Transactions)
                    .ThenInclude(e=>e.Session)
                .Include(e=>e.Transactions)
                    .ThenInclude(e=>e.ReferenceTransaction)
                    .ThenInclude(e=>e.Location)
                .Where(e=>e.PartId==this._selectedPart.Id)
                .ToListAsync();

            this.IsBubbler = this.SelectedPart.HoldsBubblers;
            this.IsNotBubbler = !this.IsBubbler;

            this.PartInstances =new ObservableCollection<PartInstance>(instances);

            var attachments = this._context.Attachments.Where(e => e.PartId == this.SelectedPart.Id);
            this.Attachments = new ObservableCollection<Attachment>(attachments);

            var warehouses =await this._context.Locations.AsNoTracking().OfType<Warehouse>().ToListAsync();
            this.Warehouses = new ObservableCollection<Warehouse>(warehouses);
            this.DispatcherService.BeginInvoke(() => {
                if (this._selectedPart.Warehouse != null) {
                    this.SelectedWarehouse = warehouses.FirstOrDefault(e => e.Id == this._selectedPart.WarehouseId);
                }
            });

            var usageList = await this._context.Categories.AsNoTracking().OfType<Usage>().ToListAsync();
            this.DispatcherService.BeginInvoke(() => {

                if (this._selectedPart.Usage != null) {
                    this.SelectedUsage = usageList.FirstOrDefault(e => e.Id == this._selectedPart.UsageId);
                }

            });
            var orgs =await this._context.Categories.AsNoTracking().OfType<Organization>().ToListAsync();
            this.Organizations = new ObservableCollection<Organization>(orgs);
            if (this._selectedPart.Organization != null) {
                this.SelectedOrganization = orgs.FirstOrDefault(e => e.Id==this._selectedPart.Id);
            }



            var transactions = (from instance in this.PartInstances
                                from transaction in instance.Transactions
                                select transaction).ToList();

            this.Transactions = new ObservableCollection<Transaction>(transactions);


            //var transactions=this._context.Transactions.Include(e=>e.)
            
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var part = navigationContext.Parameters[ParameterKeys.SelectedPart] as Part;
            var isNew = Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsNew]);
            var isEdit =Convert.ToBoolean(navigationContext.Parameters[ParameterKeys.IsEdit]);
            if (part is Part) {
                this.SelectedPart = part;
                this._isEdit = isEdit;
                this._isNewPart = isNew;
                this.Visibility = (isEdit || isNew) ? Visibility.Visible : Visibility.Collapsed;
                this._eventAggregator.GetEvent<LoadPartDetailsEvent>().Publish();
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            var part = navigationContext.Parameters[ParameterKeys.SelectedPart] as Part;
            if (part is Part) {
                return this.SelectedPart != null && this.SelectedPart.Id != part.Id;
            } else {
                return true;
            }
        }
        
        public override void OnNavigatedFrom(NavigationContext navigationContext) { 
        
        }
    }
}
