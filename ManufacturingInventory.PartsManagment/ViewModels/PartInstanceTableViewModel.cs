using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;
using ManufacturingInventory.PartsManagment.Internal;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartInstanceTableViewModel : InventoryViewModelBase {

        protected IExportService PartInstanceTableExportService { get => ServiceContainer.GetService<IExportService>("PartInstanceTableExportService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PartInstanceTableDispatcherService"); }

        private ObservableCollection<PartInstance> _partInstances;
        private PartInstance _selectedInstance;
        private int _selectedPartId;
        private bool _isEdit;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        public AsyncCommand InitializeCommand { get; private set;  }
        public AsyncCommand<string> ExportTransactionsCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewInstanceDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditInstanceCommand { get; private set; }

        public PartInstanceTableViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,ManufacturingContext context) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._isEdit = false;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<PartInstance> PartInstances { 
            get => this._partInstances; 
            set => this._partInstances = value; 
        }

        public PartInstance SelectedInstance { 
            get => this._selectedInstance;
            set => SetProperty(ref this._selectedInstance,value); 
        }

        public int SelectedPartId { 
            get => this._selectedPartId;
            set => SetProperty(ref this._selectedPartId, value);
        }



        private void ViewInstanceDetailsHandler() {
            if (this._selectedInstance != null) {
                NavigationParameters parameters= new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedPartInstance, this.SelectedInstance);
                parameters.Add(ParameterKeys.IsEdit, this._isEdit);
                this._regionManager.RequestNavigate(Regions.PartInstanceDetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
            }
        }

        private void EditInstanceHandler() {
            if (this._selectedInstance != null) {
                this._isEdit = true;
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.SelectedPartInstance, this.SelectedInstance);
                parameters.Add(ParameterKeys.IsEdit, this._isEdit);
                this._regionManager.RequestNavigate(Regions.PartInstanceDetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
            }
        }

        private async Task ExportPartInstancesHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.PartInstanceTableExportService.Export(file, format);
                    }
                    Process.Start(path);
                });
            });
        }

        private async Task InitializeHandler() {
            var partInstances = await this._context.PartInstances
                .Include(e => e.Transactions)
                .ThenInclude(e => e.Session)
                .Include(e => e.PartType)
                .Include(e => e.CurrentLocation)
                .Include(e => e.Price)
                .Include(e => e.BubblerParameter)
                .Include(e => e.Condition)
                .Where(e => e.PartId == this.SelectedPartId).ToListAsync();
            this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
        }
    }
}
