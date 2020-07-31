using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
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
using ManufacturingInventory.Reporting.Internal;

namespace ManufacturingInventory.Reporting.ViewModels {
    public class ReportingMainViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatherService { get => ServiceContainer.GetService<IDispatcherService>("ReportingMainDispatcher"); }
        //protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("ReportMainMessageBoxService"); }

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        public AsyncCommand<string> LoadReportsViewCommand { get; set; }

        public ReportingMainViewModel(IRegionManager regionManager,IEventAggregator eventAggregator) {
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.LoadReportsViewCommand = new AsyncCommand<string>(this.LoadModuleHandler);
        }

        public override bool KeepAlive => true;

        private async Task LoadModuleHandler(string navigationPath) {
            await Task.Run(() => {
                if (!string.IsNullOrEmpty(navigationPath)) {
                    this.DispatherService.BeginInvoke(() => {
                        this._regionManager.RequestNavigate(LocalRegions.ReportingMainRegion, navigationPath);
                    });
                }           
            });
        }
    }
}
