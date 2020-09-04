using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.PartInstanceTableView;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Microsoft.EntityFrameworkCore;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private bool _displayReusable;
        private bool _returnInProgress;
        private string _returnItemToolTip;
        private string _outgoingItemToolTip;
        private string _checkInExistingToolTip;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPartInstanceTableViewUseCase _partInstanceView;

        public AsyncCommand InitializeCommand { get; private set;  }
        public AsyncCommand<ExportFormat> ExportTableCommand { get; private set; }
        public AsyncCommand AddToOutgoingCommand { get; private set; }
        public AsyncCommand ViewInstanceDetailsCommand { get; private set; }
        public AsyncCommand EditInstanceCommand { get; private set; }
        public AsyncCommand CheckInCommand { get; private set; }
        public AsyncCommand CheckInExisitingCommand { get; private set; }
        public AsyncCommand ReturnItemCommand { get; private set; }
        public AsyncCommand DoubleClickViewCommand { get; private set; }

        public PartInstanceTableViewModel(IPartInstanceTableViewUseCase partInstanceView, IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._partInstanceView = partInstanceView;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.ViewInstanceDetailsCommand = new AsyncCommand(this.ViewInstanceDetailsHandlerAsync,this.CanExecuteView);
            this.EditInstanceCommand = new AsyncCommand(this.EditInstanceHandler,this.CanExecuteEdit);
            this.AddToOutgoingCommand = new AsyncCommand(this.AddToOutgoingHandler,this.CanExecuteOutgoing);
            this.CheckInCommand = new AsyncCommand(this.CheckInHandler,this.CanExecuteCheckIn);
            this.ExportTableCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);
            this.CheckInExisitingCommand = new AsyncCommand(this.CheckInExisitingHandler,this.CanExecuteAddToExisting);
            this.DoubleClickViewCommand = new AsyncCommand(this.ViewInstanceDetailsHandlerAsync,this.CanExecuteView);
            this.ReturnItemCommand = new AsyncCommand(this.ReturnItemHandler, this.CanReturnItem);

            this._eventAggregator.GetEvent<ReloadEvent>().Subscribe(async () => await this.ReloadNoTraveler());
            this._eventAggregator.GetEvent<PartInstanceEditDoneEvent>().Subscribe(async (traveler) => await this.PartInstanceEditDoneEvent(traveler));
            this._eventAggregator.GetEvent<PartInstanceEditCancelEvent>().Subscribe(async (traveler) => await this.PartInstanceEditCancelHandler(traveler));
            this._eventAggregator.GetEvent<OutgoingDoneEvent>().Subscribe(async (instanceIds) => await this.OutGoingDoneHandler(instanceIds));
            this._eventAggregator.GetEvent<OutgoingCancelEvent>().Subscribe(async () => await this.OutgoingCancelHandler());
            this._eventAggregator.GetEvent<CheckInDoneEvent>().Subscribe(async (instanceId) => await this.CheckInDoneHandler(instanceId));
            this._eventAggregator.GetEvent<CheckInCancelEvent>().Subscribe(async () => await this.CheckInCanceledHandler());
            this._eventAggregator.GetEvent<PriceEditDoneEvent>().Subscribe(async () => await this.PriceEditDoneHandler());
            this._eventAggregator.GetEvent<ReturnDoneEvent>().Subscribe(async (instanceId) => { await this.ReturnDoneHandler(instanceId); });
            this._eventAggregator.GetEvent<ReturnCancelEvent>().Subscribe(async () => { await this.ReturnCancelHandler();});
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

        public bool DisplayReusable { 
            get => this._displayReusable; 
            set=>SetProperty(ref this._displayReusable,value);
        }
        
        public string ReturnItemToolTip { 
            get => this._returnItemToolTip;
            set => SetProperty(ref this._returnItemToolTip, value);
        }
        
        public string OutgoingItemToolTip { 
            get => this._outgoingItemToolTip;
            set => SetProperty(ref this._outgoingItemToolTip, value);
        }
        
        public string CheckInExistingToolTip { 
            get => this._checkInExistingToolTip;
            set => SetProperty(ref this._checkInExistingToolTip, value);
        }

        #endregion

        #region RequestNavigationRegion

        private void ViewInstanceDetailsHandler() {
            if (this.SelectedPartInstance != null) {
                this.CleanupRegions();
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
                parameters.Add(ParameterKeys.IsEdit, false);
                parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
            }
        }

        private async Task ViewInstanceDetailsHandlerAsync() {
            if (this.SelectedPartInstance != null) {
                await Task.Run(() => {
                    this.DispatcherService.BeginInvoke(() => {
                        this.CleanupRegions();
                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
                        parameters.Add(ParameterKeys.IsEdit, false);
                        parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                        parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                        this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
                    });
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

        private async Task EditInstanceHandler() {
            if (this.SelectedPartInstance != null) {
                await Task.Run(() => {
                    this.DispatcherService.BeginInvoke(() => {
                        this.CleanupRegions();
                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
                        parameters.Add(ParameterKeys.IsEdit, true);
                        parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                        parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                        this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
                        this._editInProgess = true;
                    });
                });
            }
        }

        private async Task CheckInHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    this._checkInInProgress = true;
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    parameters.Add(ParameterKeys.IsExisiting, false);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.CheckInView, parameters);
                });
            });
        }

        private async Task CheckInExisitingHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    this._checkInInProgress = true;
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    parameters.Add(ParameterKeys.IsExisiting, true);
                    parameters.Add(ParameterKeys.InstanceId, this.SelectedPartInstance.Id);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.CheckInView, parameters);
                });
            });
        }

        private async Task ReturnItemHandler() {
            var lastOutgoing = await this._partInstanceView.GetLastOutgoing(this.SelectedPartInstance.Id);
            if (lastOutgoing != null) {
                this.DispatcherService.BeginInvoke(() => {
                    this._returnInProgress = true;
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedTransaction,lastOutgoing);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.ReturnItemView, parameters);
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Error: Unable to return selected item","Error",MessageButton.OK,MessageIcon.Error);
                });
            }
        }

        #endregion

        #region buttonCanEdits

        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
        }

        public bool CanExecuteView() {
            return (!this._editInProgess && !this._checkInInProgress && !this._returnInProgress && !this._outgoingInProgress);
        }

        public bool CanExecuteAddToExisting() {
            return (!this._editInProgess && !this._checkInInProgress && !this._outgoingInProgress && !this._returnInProgress)
                    && (!this.SelectedPartInstance.IsReusable && !this.SelectedPartInstance.IsBubbler);
        }

        public bool CanExecuteOutgoing() {
            if (this.SelectedPartInstance.IsBubbler || this.SelectedPartInstance.IsReusable) {
                return (!this._editInProgess && !this._checkInInProgress && !this._returnInProgress)
                && ((!this.SelectedPartInstance.DateInstalled.HasValue && !this.SelectedPartInstance.DateRemoved.HasValue)
                        || (this.SelectedPartInstance.DateInstalled.HasValue && this.SelectedPartInstance.DateRemoved.HasValue));
            } else {
                return (!this._editInProgess && !this._checkInInProgress && !this._returnInProgress);
            }
        }

        public bool CanReturnItem() {
            return (!this._editInProgess && !this._checkInInProgress && !this._outgoingInProgress && !this._returnInProgress)
                && (this.SelectedPartInstance.IsReusable || this.SelectedPartInstance.IsBubbler)
                && (this.SelectedPartInstance.DateInstalled.HasValue && !this.SelectedPartInstance.DateRemoved.HasValue);
        }

        public bool CanExecuteEdit() {
            return (!this._editInProgess && !this._checkInInProgress && !this._outgoingInProgress && !this._returnInProgress);
        }

        public bool CanExecuteCheckIn() {
            return (!this._editInProgess && !this._checkInInProgress && !this._outgoingInProgress && !this._returnInProgress);
        }

        #endregion

        #region CallbackRegion

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

        private async Task PriceEditDoneHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            var bubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = bubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.SelectedPartInstance = null;
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

        private async Task ReturnDoneHandler(int instanceId) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);          
            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            var isbubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = isbubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == instanceId);
                this.ShowTableLoading = false;
                this.ViewInstanceDetailsHandler();
            });
            this._returnInProgress = false;
        }

        private async Task ReturnCancelHandler() {
            this._returnInProgress = false;
            await this.ReloadNoTraveler();
        }

        #endregion

        #region LoadRegion

        private async Task InitializeHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.DisplayReusable = this.IsBubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.ShowTableLoading = false;
            });
        }

        private async Task ReloadHandler(ReloadEventTraveler traveler) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.DisplayReusable = this.IsBubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == traveler.PartInstanceId);
                this.ShowTableLoading = false;
                this.ViewInstanceDetailsHandler();
            });
        }

        private async Task ReloadNoTraveler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            await this._partInstanceView.Load();
            var partInstances = await this._partInstanceView.GetPartInstances(this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                //this.IsBubbler = isBubbler;
                this.DisplayReusable = this.IsBubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                if (this.SelectedPartInstance != null) {
                    this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == this.SelectedPartInstance.Id);
                    this.ViewInstanceDetailsHandler();
                } else {
                    this.CleanupRegions();
                }
                this.ShowTableLoading = false;
            });
        }

        #endregion

    }
}
