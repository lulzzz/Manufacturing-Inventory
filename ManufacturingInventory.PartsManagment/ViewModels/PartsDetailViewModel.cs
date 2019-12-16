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
        private bool outGoingInProgress = false;
        private bool isNewProduct = false;
        private bool isEditProduct = false;
        private bool editInProgress = false;
        private Part _selectedPart;
        private ObservableCollection<PartInstance> _partInstances = new ObservableCollection<PartInstance>();
        private Visibility _visibility;

        public AsyncCommand UndoTransactionCommand { get; set; }
        public AsyncCommand ViewInstanceDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditInstanceCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewInstanceCommand { get; private set; }

        public PartsDetailViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ManufacturingContext context) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

        }

        public override bool KeepAlive => false;

        public Part SelectedPart { 
            get => this._selectedPart;
            set => SetProperty(ref this._selectedPart,value); 
        }
        public ObservableCollection<PartInstance> PartInstances { 
            get => this._partInstances; 
            set => this._partInstances = value; 
        }

        private async Task LoadAsync() {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var part = navigationContext.Parameters["SelectedPart"] as Part;
            this.SelectedPart = part;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        
        public override void OnNavigatedFrom(NavigationContext navigationContext) { 
        
        }
    }
}
