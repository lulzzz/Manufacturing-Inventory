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
using ManufacturingInventory.Domain.Extensions;
using ManufacturingInventory.Application.Boundaries;
using System.Text;
using ManufacturingInventory.Infrastructure.Model;

namespace ManufacturingInventory.Reporting.ViewModels {
    public class ReportingMonthlySummaryViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("MonthlySummaryDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("MonthlySummaryMessageBoxService"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ManufacturingContext _context;

        private ObservableCollection<ReportSnapshot> _reportSnapshot;

        public AsyncCommand CollectSnapshotCommand { get; private set; }

        public ReportingMonthlySummaryViewModel(IRegionManager regionManager,IEventAggregator eventAggregator,ManufacturingContext context) {
            this._context = context;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
        }

        public override bool KeepAlive => true;
        
        public ObservableCollection<ReportSnapshot> ReportSnapshot {
            get => this._reportSnapshot;
            set => SetProperty(ref this._reportSnapshot, value);
        }

        private Task CollectSummaryHandler() {
            return Task.CompletedTask;
        }
    
    }
}
