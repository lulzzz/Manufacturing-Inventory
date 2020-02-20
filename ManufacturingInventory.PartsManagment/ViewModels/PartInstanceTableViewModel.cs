using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Application.Boundaries.PartInstanceTableView;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartInstanceTableViewModel : InventoryViewModelBase {

        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("PartInstanceTableExportService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PartInstanceTableDispatcher"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PartInstanceTableMessageBoxService"); }

        private ObservableCollection<PartInstance> _partInstances;       
        private PartInstance _selectedInstance;
        private int _selectedPartId;
        private bool _outgoingInProgress;
        private bool _checkInInProgress;
        private bool _editInProgess;
        private bool _showTableLoading;
        private bool _isBubbler;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPartInstanceTableViewUseCase _partInstanceView;

        public AsyncCommand InitializeCommand { get; private set;  }
        public AsyncCommand<ExportFormat> ExportTableCommand { get; private set; }
        public AsyncCommand AddToOutgoingCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewInstanceDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditInstanceCommand { get; private set; }
        public AsyncCommand CheckInCommand { get; private set; }


        public PartInstanceTableViewModel(IPartInstanceTableViewUseCase partInstanceView, IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._partInstanceView = partInstanceView;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.ViewInstanceDetailsCommand = new PrismCommands.DelegateCommand(this.ViewInstanceDetailsHandler);
            this.EditInstanceCommand = new PrismCommands.DelegateCommand(this.EditInstanceHandler);
            this.AddToOutgoingCommand = new AsyncCommand(this.AddToOutgoingHandler);
            this.CheckInCommand = new AsyncCommand(this.CheckInHandler);
            this.ExportTableCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);

            this._eventAggregator.GetEvent<PartInstanceEditDoneEvent>().Subscribe(async (traveler) => await this.PartInstanceEditDoneEvent(traveler));
            this._eventAggregator.GetEvent<PartInstanceEditCancelEvent>().Subscribe(async (traveler) => await this.PartInstanceEditCancelHandler(traveler));
            this._eventAggregator.GetEvent<OutgoingDoneEvent>().Subscribe(async (instanceIds) => await this.OutGoingDoneHandler(instanceIds));
            this._eventAggregator.GetEvent<OutgoingCancelEvent>().Subscribe(async () => await this.OutgoingCancelHandler());
            this._eventAggregator.GetEvent<CheckInDoneEvent>().Subscribe(async (instanceId) => await this.CheckInDoneHandler(instanceId));
            this._eventAggregator.GetEvent<CheckInCancelEvent>().Subscribe(async () => await this.CheckInCanceledHandler());
           
        }

        #region BindingVariables

        public override bool KeepAlive => false;

        public ObservableCollection<PartInstance> PartInstances { 
            get => this._partInstances;
            set => SetProperty(ref this._partInstances, value);
        }

        public PartInstance SelectedPartInstance { 
            get => this._selectedInstance;
            set => SetProperty(ref this._selectedInstance,value); 
        }

        public int SelectedPartId { 
            get => this._selectedPartId;
            set => SetProperty(ref this._selectedPartId, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        #endregion

        private async Task ExportTableHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportService.Export(file, format);
                    }
                    using (var process = new Process()) {
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.FileName = path;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                    }
                });
            });
        }

        private async Task InitializeHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.ShowTableLoading = false;
            });
        }

        private void EditInstanceHandler() {
            if (this.SelectedPartInstance != null) {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
                    parameters.Add(ParameterKeys.IsEdit, true);
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
                });
            }
        }

        private void ViewInstanceDetailsHandler() {
            if (this.SelectedPartInstance != null) {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
                    parameters.Add(ParameterKeys.IsEdit, false);
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
                });

            }
        }

        private async Task AddToOutgoingHandler() {
            await Task.Run(() => {
                if (this.SelectedPartInstance != null) {
                    if (this.SelectedPartInstance.Quantity > 0) {
                        if (!this._outgoingInProgress) {
                            this.DispatcherService.BeginInvoke(() => this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll());
                            this._outgoingInProgress = true;
                            this.DispatcherService.BeginInvoke(() => {
                                this.CleanupRegions();
                                NavigationParameters parameters = new NavigationParameters();
                                parameters.Add(ParameterKeys.SelectedPartInstance, this.SelectedPartInstance);
                                this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.CheckoutView, parameters);
                            });
                        } else {
                            this.DispatcherService.BeginInvoke(() => {
                                this._eventAggregator.GetEvent<AddToOutgoingEvent>().Publish(this.SelectedPartInstance);
                                this.MessageBoxService.ShowMessage("Item Added", "Success", MessageButton.OK, MessageIcon.Information);
                            });
                        }
                    } else {
                        this.DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error: Item must have a Quantity of 1 or more", "Error", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                }
            });
        }

        private async Task OutgoingCancelHandler() {
            await Task.Run(() => {
                this._outgoingInProgress = false;
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                });

            });
        }

        private async Task OutGoingDoneHandler(int firstInstanceId) {
            this._outgoingInProgress = false;
            await this.ReloadHandler(new ReloadEventTraveler() { PartId=this.SelectedPartId,PartInstanceId= firstInstanceId});
        }

        private async Task ReloadNoTraveler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);

            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            var bubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = bubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                if (this.SelectedPartInstance != null) {
                    this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == this.SelectedPartInstance.Id);
                    this.ViewInstanceDetailsHandler();
                }
                this.ShowTableLoading = false;
            });
        }

        private async Task PartInstanceEditDoneEvent(ReloadEventTraveler traveler) {
            this._editInProgess = false;
            await this.ReloadHandler(traveler);
        }

        private async Task PartInstanceEditCancelHandler(ReloadEventTraveler traveler) {
            this._editInProgess = false;
            await this.ReloadHandler(traveler);
        }

        private async Task ReloadHandler(ReloadEventTraveler traveler) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            var bubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = bubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == traveler.PartInstanceId);
                this.ShowTableLoading = false;
                this.ViewInstanceDetailsHandler();
            });
        }
        
        private async Task CheckInHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    this._checkInInProgress = true;
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.CheckInView, parameters);
                });
            });
        }

        private async Task CheckInDoneHandler(int instanceId) {
            this._checkInInProgress = false;
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            var bubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = bubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == instanceId);
                this.ShowTableLoading = false;
                this.ViewInstanceDetailsHandler();
            });
        }

        private async Task CheckInCanceledHandler() {
            this._checkInInProgress = false;
            await this.ReloadNoTraveler();
        }
    
        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
        }
    
        public bool CanExecute() {
            return !(this._editInProgess && this._checkInInProgress && this._outgoingInProgress);
        }

        public bool CanExecuteOutgoing() {
            return !(this._editInProgess && this._checkInInProgress);
        }
    }
}
