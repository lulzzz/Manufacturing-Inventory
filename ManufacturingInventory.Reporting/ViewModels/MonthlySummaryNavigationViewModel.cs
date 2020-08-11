using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Domain.Extensions;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ManufacturingInventory.Reporting.Internal;

namespace ManufacturingInventory.Reporting.ViewModels {
    public class MonthlySummaryNavigationViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("SummaryNavigationDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("SummaryNavigationMessageService"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private INavigationSummaryUseCase _reportNavigationService;
        private ObservableCollection<MonthlySummary> _monthlySummaries;
        private MonthlySummary _selectedReport;
        private bool _showTableLoading = false;
        private bool _isLoaded = false;


        public AsyncCommand NewReportCommand { get; private set; }
        public AsyncCommand LoadSelectedReportCommand { get; private set; }
        public AsyncCommand DeleteSelectedReportCommand { get; private set; }

        public MonthlySummaryNavigationViewModel(IEventAggregator eventAggregator,IRegionManager regionManager,INavigationSummaryUseCase reportNavigationService) {
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._reportNavigationService = reportNavigationService;
        }

        public override bool KeepAlive => true;

        public ObservableCollection<MonthlySummary> MonthlySummaries { 
            get => this._monthlySummaries;
            set => SetProperty(ref this._monthlySummaries, value);
        }

        public MonthlySummary SelectedReport { 
            get => this._selectedReport;
            set => SetProperty(ref this._selectedReport, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        private async Task DeleteReportHandler() {
            if (this._selectedReport != null) {
                var input = new NavigationSummaryInput(this._selectedReport, Application.Boundaries.EditAction.Delete);
                var response = await this._reportNavigationService.Execute(input);
                if (response.Success) {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                } else {
                    this.MessageBoxService.ShowMessage(response.Message,"Error",MessageButton.OK,MessageIcon.Error);
                }
            }
        }

        private async Task LoadSelectedReportHandler() {
            if (this._selectedReport != null) {
                this.DispatcherService.BeginInvoke(() =>{ 
                    NavigationParameters parameters= new NavigationParameters();
                    parameters.Add(ParameterKeys.SelectedReportId, this._selectedReport.Id);
                    this._regionManager.RequestNavigate(LocalRegions.ReportingMainRegion, ModuleViews.ReportingCurrentInventoryView, parameters);
                });
            }
        }

        private async Task DoubleClickLoadCommand() {

        }

        private async Task LoadAsync() {
            if (!this._isLoaded) {
                await this._reportNavigationService.Load();
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var reports = await this._reportNavigationService.GetExistingReports();
                this.MonthlySummaries = new ObservableCollection<MonthlySummary>(reports);
                this.DispatcherService.BeginInvoke(()=>this.ShowTableLoading=false);
                this._isLoaded = true;
            }
        }

    }
}
