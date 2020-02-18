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
        private bool _showTableLoading;
        private bool _isBubbler;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IEntityProvider<PartInstance> _provider;

        public AsyncCommand InitializeCommand { get; private set;  }
        public AsyncCommand<ExportFormat> ExportTableCommand { get; private set; }
        public AsyncCommand AddToOutgoingCommand { get; private set; }
        public PrismCommands.DelegateCommand ViewInstanceDetailsCommand { get; private set; }
        public PrismCommands.DelegateCommand EditInstanceCommand { get; private set; }
        public AsyncCommand CheckInCommand { get; private set; }

        public PartInstanceTableViewModel(IEntityProvider<PartInstance> provider, IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._provider = provider;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.ViewInstanceDetailsCommand = new PrismCommands.DelegateCommand(this.ViewInstanceDetailsHandler);
            this.EditInstanceCommand = new PrismCommands.DelegateCommand(this.EditInstanceHandler);
            this.AddToOutgoingCommand = new AsyncCommand(this.AddToOutgoingHandler);
            this.CheckInCommand = new AsyncCommand(this.CheckInHandler);
            this.ExportTableCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);

            this._eventAggregator.GetEvent<ReloadEvent>().Subscribe(async (traveler) => await this.ReloadHandler(traveler));
            this._eventAggregator.GetEvent<OutgoingDoneEvent>().Subscribe(async () => await this.OutGoingDoneHandler());
            this._eventAggregator.GetEvent<PriceEditDoneEvent>().Subscribe(async () => await this.ReloadNoTraveler());

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
            var partInstances = await this._provider.GetEntityListAsync(e => e.PartId == this.SelectedPartId);
            this.DispatcherService.BeginInvoke(() => {
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.ShowTableLoading = false;
            });
        }

        private void ViewInstanceDetailsHandler() {
            if (this.SelectedPartInstance != null) {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedPartInstance, this.SelectedPartInstance);
                    parameters.Add(ParameterKeys.IsEdit, false);
                    parameters.Add(ParameterKeys.IsNew, false);
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

        private async Task OutGoingDoneHandler() {
            this._outgoingInProgress = false;
            this.DispatcherService.BeginInvoke(() => {
                this.CleanupRegions();
            });
            await this.ReloadHandler(new ReloadEventTraveler() { PartId=this.SelectedPartId,PartInstanceId=0});
        }

        private async Task ReloadNoTraveler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);

            await this._provider.LoadAsync();
            var partInstances = await this._provider.GetEntityListAsync(e => e.PartId == this.SelectedPartId);
            var bubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = bubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                if (this.SelectedPartInstance != null) {
                    this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == this.SelectedPartInstance.Id);
                }
                this.ShowTableLoading = false;
            });
        }

        private async Task ReloadHandler(ReloadEventTraveler traveler) {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);

            await this._provider.LoadAsync();
            var partInstances = await this._provider.GetEntityListAsync(e => e.PartId == this.SelectedPartId);
            var bubbler = partInstances.Select(e => e.IsBubbler).Contains(true);

            this.DispatcherService.BeginInvoke(() => {
                this.IsBubbler = bubbler;
                this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
                this.SelectedPartInstance = this.PartInstances.FirstOrDefault(e => e.Id == traveler.PartInstanceId);
                this.ShowTableLoading = false;
                this.ViewInstanceDetailsHandler();
            });
        }

        private Task CheckInHandler() {
            return Task.Factory.StartNew(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.CheckInView, parameters);
                });
            });
        }

        private Task CheckInDoneHandler() {
            return Task.CompletedTask;
        }

        private Task CheckInCanceledHandler() {
            return Task.CompletedTask;
        }


        
        private void EditInstanceHandler() {
            if (this.SelectedPartInstance != null) {
                this.DispatcherService.BeginInvoke(() => {
                    this.CleanupRegions();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedPartInstance, this.SelectedPartInstance);
                    parameters.Add(ParameterKeys.IsEdit, true);
                    parameters.Add(ParameterKeys.IsNew, false);
                    parameters.Add(ParameterKeys.IsBubbler, this.IsBubbler);
                    parameters.Add(ParameterKeys.PartId, this.SelectedPartId);
                    this._regionManager.RequestNavigate(LocalRegions.DetailsRegion, ModuleViews.PartInstanceDetailsView, parameters);
                });
            }
        }

        public void CleanupRegions() {
            this._regionManager.Regions[LocalRegions.DetailsRegion].RemoveAll();
        }
    }
}
