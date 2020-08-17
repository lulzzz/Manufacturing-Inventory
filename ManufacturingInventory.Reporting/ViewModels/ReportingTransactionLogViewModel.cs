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
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using System.IO;
using System.Diagnostics;

namespace ManufacturingInventory.Reporting.ViewModels {
    public class ReportingTransactionLogViewModel : InventoryViewModelBase {
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("TransactionLogDispatcherService"); }
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("TransactionLogMessageBoxService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("TransactionLogExportService"); }

        private ITransactionLogUseCase _transactionService;

        private bool _isLoaded;
        private bool _showTableLoading = false;
        private ObservableCollection<Transaction> _transactions;
        private CollectType _selectedCollectionType;
        private DateTime _start;
        private DateTime _stop;

        public AsyncCommand<ExportFormat> TransactionLogExportCommand { get; private set; }
        public AsyncCommand CollectTransactionLogCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }


        public ReportingTransactionLogViewModel(ITransactionLogUseCase transactionService) {
            this._transactionService = transactionService;
            this.TransactionLogExportCommand = new AsyncCommand<ExportFormat>(this.ExportTableHandler);
            this.CollectTransactionLogCommand = new AsyncCommand(this.CollectTransactionLogHandler);
            this.InitializeCommand = new AsyncCommand(this.Load);
        }

        public override bool KeepAlive => true;

        public DateTime Start {
            get => this._start;
            set => SetProperty(ref this._start, value);
        }

        public DateTime Stop {
            get => this._stop;
            set => SetProperty(ref this._stop, value);
        }

        public CollectType SelectedCollectionType {
            get => this._selectedCollectionType;
            set => SetProperty(ref this._selectedCollectionType, value);
        }

        public bool ShowTableLoading { 
            get => this._showTableLoading; 
            set => SetProperty(ref this._showTableLoading,value);
        }

        public ObservableCollection<Transaction> Transactions { 
            get => this._transactions; 
            set => SetProperty(ref this._transactions, value);
        }

        private async Task CollectTransactionLogHandler() {
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
            TransactionSummaryInput input = new TransactionSummaryInput(this.Start, this.Stop, this.SelectedCollectionType);
            var output = await this._transactionService.Execute(input);
            if (output.Success) {
                this.Transactions = new ObservableCollection<Transaction>(output.Transactions);
            } else {
                this.MessageBoxService.ShowMessage(output.Message,"Error",MessageButton.OK,MessageIcon.Error,MessageResult.OK);
            }
            this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
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

        private async Task Load() {
            if (!this._isLoaded) {
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = true);
                await this._transactionService.Load();
                this.SelectedCollectionType = CollectType.OnlyCostReported;
                this.Start = DateTime.Now;
                this.Stop = DateTime.Now;
                this._isLoaded = true;
                this.DispatcherService.BeginInvoke(() => this.ShowTableLoading = false);
            }
        }
    }
}
