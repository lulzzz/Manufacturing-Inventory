using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
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
using System.Diagnostics;
using System.IO;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Reporting.ViewModels {
    public class ReportingMonthlySummaryViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("MonthlySummaryDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("MonthlySummaryMessageBoxService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("MonthlySummaryExportService"); }

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IMonthlySummaryUseCase _reportingService;
        private MonthlySummary _monthlySummary=new MonthlySummary();
        private bool _isLoaded = false;

        private ObservableCollection<IPartMonthlySummary> _reportSnapshot;
        private ObservableCollection<string> _existingReportList;

        private DateTime _start;
        private DateTime _stop;
        private bool _saveEnabled=false;
        private bool _showTableLoading;

        public AsyncCommand<ExportFormat> ExportTableCommand { get; private set; }
        public AsyncCommand CollectSnapshotCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set;}
        public AsyncCommand SaveMonthlyReportCommand { get; private set; }


        public ReportingMonthlySummaryViewModel(IRegionManager regionManager,IEventAggregator eventAggregator,IMonthlySummaryUseCase reportingService) {
            this._reportingService = reportingService;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this.Start = DateTime.Now;
            this.Stop = DateTime.Now;
            this.CollectSnapshotCommand = new AsyncCommand(this.CollectSummaryHandler);
            this.ExportTableCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);
            this.SaveMonthlyReportCommand = new AsyncCommand(this.SaveMonthlyReportHandler);
            this.InitializeCommand = new AsyncCommand(this.LoadAsync);
        }

        public override bool KeepAlive => true;
        
        public ObservableCollection<IPartMonthlySummary> ReportSnapshot {
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

        private async Task CollectSummaryHandler() {
            MonthlySummaryInput reportInput = new MonthlySummaryInput(this._start, this._stop);
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading=true);
            var output=await this._reportingService.Execute(reportInput);
            if (output.Success) {
                this.ReportSnapshot = new ObservableCollection<IPartMonthlySummary>(output.Snapshot.MonthlyPartSnapshots);            
                this.SaveEnabled = true;
            } else {
                this.MessageBoxService.ShowMessage(output.Message,"",MessageButton.OK,MessageIcon.Error);
            }

            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
        }

        public async Task SaveMonthlyReportHandler() {
            var saved=await this._reportingService.SaveMonthlySummary(this.MonthlySummary);
            if (saved!=null) {
                this.MonthlySummary = saved;
                this.ReportSnapshot = new ObservableCollection<IPartMonthlySummary>(this._monthlySummary.MonthlyPartSnapshots);
                RaisePropertyChanged();
                this.MessageBoxService.ShowMessage("Monthly Report Saved", "Report Saved", MessageButton.OK, MessageIcon.Information);
            } else {
                this.MessageBoxService.ShowMessage("Error:  Could not save monthly report"+Environment.NewLine+"Please trye generating report again",
                    "Error", MessageButton.OK, MessageIcon.Error);
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

        private async Task LoadAsync() {
            if (!this._isLoaded) {
                await this._reportingService.Load();
                var now = DateTime.Now;
                var monthlySummary = new MonthlySummary(now,now);
                monthlySummary.DateGenerated = now;
                monthlySummary.MonthOfReport = now.ToString("MMMM");
                var existingList = await this._reportingService.GetExistingReports();
                this.ExistingReportList = new ObservableCollection<string>(existingList);
                this.SaveEnabled = false;
                this.MonthlySummary = monthlySummary;
            }
        }
    }
}
