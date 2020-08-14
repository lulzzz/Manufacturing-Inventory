using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
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
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.Domain.Extensions;
using ManufacturingInventory.Application.Boundaries;
using System.Text;
using System.IO;
using System.Diagnostics;
using ManufacturingInventory.Reporting.Internal;

namespace ManufacturingInventory.Reporting.ViewModels {
    public class ReportingCurrentInventoryViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CurrentInventoryDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CurrentInventoryMessageBoxService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("CurrentInventoryExportService"); }

        private ICurrentInventoryUseCase _currentinventoryService;

        private ObservableCollection<CurrentInventoryItem> _currentInventory=new ObservableCollection<CurrentInventoryItem>();
        private bool _isLoaded=false;
        private bool _showTableLoading = false;
        private CollectType _selectedCollectionType;


        public AsyncCommand<ExportFormat> ExportInventoryCommand { get; private set; }
        public AsyncCommand CollectCurrentInventoryCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }


        public ReportingCurrentInventoryViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,ICurrentInventoryUseCase currentInventoryService) {
            this._currentinventoryService = currentInventoryService;
            this.ExportInventoryCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);
            this.CollectCurrentInventoryCommand = new AsyncCommand(this.CollectCurrentInventoryHandler);
            this.InitializeCommand = new AsyncCommand(this.Load);
        }

        public override bool KeepAlive => true;

        public ObservableCollection<CurrentInventoryItem> CurrentInventory { 
            get => this._currentInventory;
            set => SetProperty(ref this._currentInventory, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading; 
            set => SetProperty(ref this._showTableLoading,value);
        }

        public CollectType SelectedCollectionType { 
            get => this._selectedCollectionType; 
            set => SetProperty(ref this._selectedCollectionType,value);
        }

        public async Task CollectCurrentInventoryHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            CurrentInventoryInput input = new CurrentInventoryInput(this._selectedCollectionType);
            var output=await this._currentinventoryService.Execute(input);
            if (output.Success) {
                this.CurrentInventory = new ObservableCollection<CurrentInventoryItem>(output.CurrentInventoryItems);
            } else {
                this.MessageBoxService.ShowMessage(output.Message,"Error",MessageButton.OK,MessageIcon.Error,MessageResult.OK);
            }
            this.DispatcherService.BeginInvoke(() =>this.ShowTableLoading=false);
        }

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

        public async Task Load() {
            if (!this._isLoaded) {
                await this._currentinventoryService.Load();
                this.SelectedCollectionType = CollectType.OnlyCostReported;
                this._isLoaded = true;
            }
        }

    }
}
