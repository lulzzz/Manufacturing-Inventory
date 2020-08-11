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

namespace ManufacturingInventory.Reporting.ViewModels {
    public class ReportingMonthlySummaryViewModel : InventoryViewModelNavigationBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("MonthlySummaryDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("MonthlySummaryMessageBoxService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("MonthlySummaryExportService"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IMonthlySummaryUseCase _reportingService;
        private MonthlySummary _monthlySummary=new MonthlySummary();
        private bool _isLoaded = false;

        private ObservableCollection<PartMonthlySummary> _reportSnapshot;
        private ObservableCollection<string> _existingReportList;

        private string _monthOfReport;
        private DateTime _start;
        private DateTime _stop;
        private string _selectedMonth;
        private bool _saveEnabled=false;
        private bool _showTableLoading;
        private string _saveButtonText;
        private bool _existingReportLoaded = false;

        public AsyncCommand<ExportFormat> ExportTableCommand { get; private set; }
        public AsyncCommand CollectSnapshotCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set;}
        public AsyncCommand SaveMonthlyReportCommand { get; private set; }
        public AsyncCommand LoadExisitingReportCommand { get; private set; }


        public ReportingMonthlySummaryViewModel(IRegionManager regionManager,IEventAggregator eventAggregator,IMonthlySummaryUseCase reportingService) {
            this._reportingService = reportingService;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            var now=DateTime.Now;
            this.Start = now;
            this.Stop = now;
            this.MonthOfReport = now.MonthName();
            this.CollectSnapshotCommand = new AsyncCommand(this.CollectSummaryHandler);
            this.ExportTableCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);
            this.SaveMonthlyReportCommand = new AsyncCommand(this.SaveMonthlyReportHandler);
            this.LoadExisitingReportCommand = new AsyncCommand(this.LoadExisitingHandler);
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
        }

        public override bool KeepAlive => true;
        
        public ObservableCollection<PartMonthlySummary> ReportSnapshot {
            get => this._reportSnapshot;
            set => SetProperty(ref this._reportSnapshot, value);
        }

        public DateTime Start {
            get => this._start;
            set => SetProperty(ref this._start, value);
        }

        public DateTime Stop {
            get => this._stop;
            set => SetProperty(ref this._stop, value);
        }

        public bool ShowTableLoading {
            get => this._showTableLoading;
            set => SetProperty(ref this._showTableLoading, value);
        }

        public bool SaveEnabled {
            get => this._saveEnabled;
            set => SetProperty(ref this._saveEnabled, value);
        }
        
        public MonthlySummary MonthlySummary { 
            get => this._monthlySummary; 
            set => SetProperty(ref this._monthlySummary,value);
        }

        public ObservableCollection<string> ExistingReportList { 
            get => this._existingReportList; 
            set => SetProperty(ref this._existingReportList,value); 
        }

        public string MonthOfReport { 
            get => this._monthOfReport; 
            set => SetProperty(ref this._monthOfReport,value);
        }

        public string SelectedMonth { 
            get => this._selectedMonth; 
            set=>SetProperty(ref this._selectedMonth, value);
        }
        public string SaveButtonText {
            get => this._saveButtonText;
            set => SetProperty(ref this._saveButtonText, value);
        }

        private async Task CollectSummaryHandler() {
            MonthlySummaryInput reportInput = new MonthlySummaryInput(this._start, this._stop);
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading=true);
            var output=await this._reportingService.Execute(reportInput);
            if (output.Success) {
                this.ReportSnapshot = new ObservableCollection<PartMonthlySummary>(output.Snapshot.MonthlyPartSnapshots);            
                this.SaveEnabled = true;
            } else {
                this.MessageBoxService.ShowMessage(output.Message,"",MessageButton.OK,MessageIcon.Error);
            }
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
        }

        public async Task SaveMonthlyReportHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            this.MonthlySummary.DateGenerated = DateTime.Now;
            this.MonthlySummary.MonthStartDate = this.Start;
            this.MonthlySummary.MonthStopDate = this.Stop;
            //this.MonthlySummary.MonthOfReport = this.MonthOfReport.MonthName();
            this.MonthlySummary.MonthOfReport = this.MonthOfReport;
            this.MonthlySummary.MonthlyPartSnapshots = this.ReportSnapshot;
            var saved=await this._reportingService.SaveMonthlySummary(this.MonthlySummary);
            if (saved!=null) {
                this.MessageBoxService.ShowMessage("Monthly Report Saved", "Report Saved", MessageButton.OK, MessageIcon.Information);
                await this.ReloadAsync(saved);
            } else {
                this.MessageBoxService.ShowMessage("Error:  Could not save monthly report"+Environment.NewLine+"Please try generating report again",
                    "Error", MessageButton.OK, MessageIcon.Error);
                
            }
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
        }

        public async Task LoadExisitingHandler() {
            if (!string.IsNullOrEmpty(this._selectedMonth)) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                var summary=await this._reportingService.LoadExisitingReport(this._selectedMonth);
                if (summary != null) {
                    await this.ReloadAsync(summary);
                    this.SaveButtonText = "Overwrite Existing Report";
                } else {
                    this.MessageBoxService.ShowMessage("No Month Selected" + Environment.NewLine + "Please select a month and try again",
                        "Selection Error", MessageButton.OK, MessageIcon.Warning);
                }
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
            } else {
                this.MessageBoxService.ShowMessage("No Month Selected"+Environment.NewLine+"Please select a month and try again",
                    "Selection Error", MessageButton.OK, MessageIcon.Warning);
            }
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

        private async Task ReloadAsync(MonthlySummary monthlySummary) {
            await this._reportingService.Load();
            this.MonthlySummary = monthlySummary;
            this.ReportSnapshot = new ObservableCollection<PartMonthlySummary>(monthlySummary.MonthlyPartSnapshots);
            this.Start = monthlySummary.MonthStartDate;
            this.Stop = monthlySummary.MonthStopDate;
            //this.MonthOfReport = new DateTime(this.Start.Year, Convert.ToDateTime(monthlySummary.MonthOfReport + " 01,1900").Month, this.Start.Day);
            this.MonthOfReport = monthlySummary.MonthOfReport;
            var existingList = await this._reportingService.GetExistingReports();
            this.ExistingReportList = new ObservableCollection<string>(existingList);
        }
        
        private async Task LoadAsync() {
            if (!this._isLoaded) {
                await this._reportingService.Load();
                this.SaveButtonText = "Save Report";
                var now = DateTime.Now;
                this.MonthOfReport = now.MonthName();
                var monthlySummary = new MonthlySummary(now,now);
                monthlySummary.DateGenerated = now;
                monthlySummary.MonthOfReport = now.MonthName();
                var existingList = await this._reportingService.GetExistingReports();
                this.ExistingReportList = new ObservableCollection<string>(existingList);
                this.SaveEnabled = false;
                this.MonthlySummary = monthlySummary;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) => throw new NotImplementedException();
        public override bool IsNavigationTarget(NavigationContext navigationContext) => throw new NotImplementedException();
        public override void OnNavigatedFrom(NavigationContext navigationContext) => throw new NotImplementedException();
    }
}
