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
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Domain.DTOs;
using System.ComponentModel;
using ManufacturingInventory.Common.Application.ValidationRules;
using System.Windows;
using Condition = ManufacturingInventory.Infrastructure.Model.Entities.Condition;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class CheckoutViewModel : InventoryViewModelNavigationBase, IDataErrorInfo {
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("CheckoutMessageBoxService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("CheckoutDispatcherService"); }
        protected IExportService ExportService { get => ServiceContainer.GetService<IExportService>("ExportOutgoingService"); }

        private IEventAggregator _eventAggregator;
        private IUserService _userService;
        private ICheckOutUseCase _checkOut;

        private string _quantityLabel;

        private ObservableCollection<TransactionDTO> _transactions=new ObservableCollection<TransactionDTO>();
        private ObservableCollection<Consumer> _consumers;
        private ObservableCollection<Condition> _conditions;
        private TransactionDTO _selectedTransaction;
        private PartInstance _selectedPartInstance;
        private Condition _selectedCondition;
        private Consumer _selectedConsumer;
        private DateTime _timeStamp;

        private double _measuredWeight;
        private double _weight;
        private double _totalCost;
        private double _unitCost;



        private int _quantity;
        private bool _isBubbler = false;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand<ExportFormat> ExportOutgoingCommand { get; private set; }
        public AsyncCommand AddToOutgoingCommand { get; private set; }
        public AsyncCommand RemoveFromOutgoingCommand { get; private set; }
        public AsyncCommand CheckOutCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }



        public CheckoutViewModel(ICheckOutUseCase checkOut,IEventAggregator eventAggregator) {
            this._checkOut = checkOut;
            this._eventAggregator = eventAggregator;
            this.TimeStamp = DateTime.Now;
            this.InitializeCommand = new AsyncCommand(this.LoadHandler);
            this.CheckOutCommand = new AsyncCommand(this.CheckOutHandler);
            this.AddToOutgoingCommand = new AsyncCommand(this.AddToOutgoingHandler,this.CanAdd);
            this.RemoveFromOutgoingCommand = new AsyncCommand(this.RemoveFromOutgoingHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);
            this.ExportOutgoingCommand = new AsyncCommand<ExportFormat>(this.ExportOutputHandler);

            this._eventAggregator.GetEvent<AddToOutgoingEvent>().Subscribe(this.RecievePartInstance);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<TransactionDTO> Transactions { 
            get => this._transactions;
            set => SetProperty(ref this._transactions, value);
        }

        public ObservableCollection<Consumer> Consumers {
            get => this._consumers;
            set => SetProperty(ref this._consumers, value);
        }

        public ObservableCollection<Condition> Conditions {
            get => this._conditions;
            set => SetProperty(ref this._conditions, value);
        }

        public TransactionDTO SelectedTransaction { 
            get => this._selectedTransaction;
            set => SetProperty(ref this._selectedTransaction, value);
        }

        public PartInstance SelectedPartInstance { 
            get => this._selectedPartInstance;
            set => SetProperty(ref this._selectedPartInstance, value);
        }

        public bool IsBubbler { 
            get => this._isBubbler;
            set => SetProperty(ref this._isBubbler, value);
        }

        public string QuantityLabel { 
            get => this._quantityLabel;
            set => SetProperty(ref this._quantityLabel, value);
        }

        public double MeasuredWeight { 
            get => this._measuredWeight;
            set {
                if (this.SelectedPartInstance != null) {
                    if (this.IsBubbler) {
                        this.SelectedPartInstance.UpdateWeight(value);
                        this.Weight = this.SelectedPartInstance.BubblerParameter.Weight;
                    }
                }
                this.SetProperty(ref this._measuredWeight, value);
            }
        }

        public double Weight {
            get => this._weight;
            set => SetProperty(ref this._weight, value);
        }

        public double UnitCost {
            get => this._unitCost;
            set => SetProperty(ref this._unitCost, value);
        }

        public double TotalCost { 
            get => this._totalCost;
            set => SetProperty(ref this._totalCost,value);
        }

        public int Quantity { 
            get => this._quantity;
            set {
                if (this.SelectedPartInstance != null) {
                    if (!this.IsBubbler) {
                        this.TotalCost = this.SelectedPartInstance.UnitCost * value;
                    } else {
                            this.TotalCost = this.SelectedPartInstance.TotalCost;
                    }
                }
                SetProperty(ref this._quantity, value);
            }
        }

        public Consumer SelectedConsumer { 
            get => this._selectedConsumer;
            set => SetProperty(ref this._selectedConsumer, value);
        }

        public Condition SelectedCondition { 
            get => this._selectedCondition;
            set => SetProperty(ref this._selectedCondition, value);
        }

        public DateTime TimeStamp { 
            get => this._timeStamp;
            set => SetProperty(ref this._timeStamp, value);
        }

        public string this[string columnName] {
            get {
                string quantityProp = BindableBase.GetPropertyName(() => this.Quantity);
                string measuredProp = BindableBase.GetPropertyName(() => this.MeasuredWeight);
                string locationProp = BindableBase.GetPropertyName(() => this.SelectedConsumer);

                if (columnName == locationProp) {
                    if (this.SelectedConsumer == null)
                        return "Consumer must be selected";
                }else if (columnName == quantityProp) {
                    if ((this.Quantity == 0 || this.Quantity>this.SelectedPartInstance.Quantity) && !this.IsBubbler)
                        return "Quantity must be > 0 and <= Stock";
                } else if (columnName == measuredProp) {
                    if (this.IsBubbler) {
                        if (this.MeasuredWeight == 0 || this.MeasuredWeight>this.SelectedPartInstance.BubblerParameter.Measured)
                            return "Measured must be > 0 and <= Current Measured Stock";
                    }
                }
                return null;
            }
        }

        private void OnValidationFailed(string error) {
            this.MessageBoxService.ShowMessage("Check In Failed. " + Environment.NewLine + error, "Error", MessageButton.OK, MessageIcon.Error);
        }

        private string EnableValidationAndGetError() {
            //this._validationEnabled = true;
            string error = ((IDataErrorInfo)this).Error;
            if (!string.IsNullOrEmpty(error)) {
                this.RaisePropertyChanged();
                return error;
            }
            return null;
        }

        public string Error {
            get {
                IDataErrorInfo me = (IDataErrorInfo)this;
                string error =me[BindableBase.GetPropertyName(() => this.Quantity)]+
                              me[BindableBase.GetPropertyName(() => this.MeasuredWeight)]+
                              me[BindableBase.GetPropertyName(() => this.SelectedConsumer)];
                if (!string.IsNullOrEmpty(error))
                    return "Please check inputs, 1 or more required items missing";
                return null;
            }
        }

        private void RecievePartInstance(PartInstance instance) {
            this.TimeStamp = DateTime.Now;
            this.SelectedPartInstance = instance;
            this.IsBubbler = instance.IsBubbler;
            this.QuantityLabel = (instance.IsBubbler) ? "Quantity" : "Enter Quantity";

            if (this.SelectedPartInstance.CostReported) {
                this.UnitCost = this.SelectedPartInstance.UnitCost;
                this.TotalCost = this.SelectedPartInstance.TotalCost;
            } else {
                this.UnitCost = 0;
                this.TotalCost = 0;
            }

            if (instance.IsBubbler) {
                this.Quantity = instance.Quantity;
            } else {
                this.Quantity = 0;
            }
        }

        private async Task AddToOutgoingHandler() {
            await Task.Run(() => {
            string error = this.EnableValidationAndGetError();
                if (error != null) {
                    this.DispatcherService.BeginInvoke(() => {
                        this.OnValidationFailed(error);
                    });
                } else {
                    var transaction = this.Transactions.FirstOrDefault(e => e.PartInstanceId == this.SelectedPartInstance.Id);
                    if (transaction == null) {
                        int conditionId = (this.SelectedCondition != null) ? this.SelectedCondition.Id : 0;

                        TransactionDTO newTransaction = new TransactionDTO(this.TimeStamp,
                            InventoryAction.OUTGOING, this.Quantity, false, this.UnitCost,
                            this.TotalCost, this.SelectedPartInstance.Id, this.SelectedPartInstance.Name,
                            this.SelectedConsumer.Name, this.SelectedConsumer.Id, this.IsBubbler,
                            this.MeasuredWeight, this.Weight, conditionId: conditionId);

                        DispatcherService.BeginInvoke(() => {
                            this.Transactions.Add(newTransaction);
                            //this.MessageBoxService.ShowMessage("Item added to Output: "+newTransaction.Weight, "Success");
                            this.SelectedPartInstance = null;
                            this.SelectedConsumer = null;
                            this.Quantity = 0;
                            this.MeasuredWeight = 0;
                            this.TimeStamp = DateTime.Now;
                        });

                    } else {
                        this.DispatcherService.BeginInvoke(() => {
                            this.MessageBoxService.ShowMessage("Error: Outgoing Already Contains Item", "Error", MessageButton.OK, MessageIcon.Error);
                        });
                    }
                }
            });
        }

        private async Task RemoveFromOutgoingHandler() {
            if (this.SelectedTransaction != null) {
                await Task.Run(() => {
                    this.DispatcherService.BeginInvoke(() => {
                        if (this.Transactions.Remove(this.SelectedTransaction)) {
                            this.MessageBoxService.ShowMessage("Item Removed from Outgoing List", "Success", MessageButton.OK, MessageIcon.Information);
                        } else {
                            this.MessageBoxService.ShowMessage("Error Removing Item From Outgoing", "Error", MessageButton.OK, MessageIcon.Error);
                        }
                    });
                });
            }
        }

        private async Task CheckOutHandler() {
            if (this._transactions.Any()) {
                CheckOutInput input = new CheckOutInput();
                await Task.Run(() => {
                    foreach (var transaction in this.Transactions) {
                        input.Items.Add(new CheckOutInputData(transaction.TimeStamp, transaction.PartInstanceId, transaction.LocationId, transaction.Quantity,
                            transaction.UnitCost, transaction.TotalCost, isBubbler: this.IsBubbler, measuredWeight: transaction.Measured, weight: transaction.Measured,
                            conditionId: transaction.ConditionId));
                    }
                });

                var response = await this._checkOut.Execute(input);

                var succeeded = response.OutputList.Where(e => e.Success == true);
                var failed = response.OutputList.Where(e => e.Success == false);

                StringBuilder okayBuilder = new StringBuilder();
                StringBuilder failBuilder = new StringBuilder();

                okayBuilder.AppendLine("Succeeded: ");

                foreach (var success in succeeded) {
                    okayBuilder.AppendLine(success.Message);
                }

                if (failed.Count() > 0) {
                    failBuilder.AppendLine("Failed: ");
                    foreach (var fail in failed) {
                        failBuilder.AppendLine(fail.Message);
                    }
                    okayBuilder.AppendLine(failBuilder.ToString());
                }

                var firstInstance = response.OutputList.Select(e => e.Transaction.PartInstanceId).First();

                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(okayBuilder.ToString(), "Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<OutgoingDoneEvent>().Publish(firstInstance);
                });
                
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage("Error:Not Items in Outgoing List"+Environment.NewLine+
                        "Check that you have added outgoing item to list","Error", MessageButton.OK,MessageIcon.Error);
                });
            }
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this._eventAggregator.GetEvent<OutgoingCancelEvent>().Publish();
                });
            });
        }

        private bool CanAdd() {
            bool canAdd = true;
            if (this.SelectedPartInstance != null) {
                if (this.IsBubbler) {
                    canAdd = (this.Weight > 0) && (this.Weight <= this.SelectedPartInstance.BubblerParameter.NetWeight);
                } else {
                    canAdd = (this.Quantity > 0) && (this.Quantity <= this.SelectedPartInstance.Quantity);
                }
            } else {
                canAdd = false;
            }
            canAdd = canAdd & (this.SelectedConsumer != null);
            return canAdd;
        }

        private async Task LoadHandler() {
            if (!this._isInitialized) {
                var consumers = await this._checkOut.GetConsumers();
                var conditions = await this._checkOut.GetConditions();
                this.DispatcherService.BeginInvoke(() => {
                    this.Consumers = new ObservableCollection<Consumer>(consumers);
                    this.Conditions = new ObservableCollection<Condition>(conditions);
                    this.SelectedConsumer = this.Consumers.FirstOrDefault(e => e.IsDefualt);
                    if (this.SelectedPartInstance != null) {
                        this.IsBubbler = this.SelectedPartInstance.IsBubbler;
                        this.QuantityLabel = (this.SelectedPartInstance.IsBubbler || this.SelectedPartInstance.IsReusable) ? "Quantity" : "Enter Quantity";
                        if (this.SelectedPartInstance.IsBubbler) {
                            this.Quantity = this.SelectedPartInstance.Quantity;
                        } else {
                            this.Quantity = 0;
                        }

                        //if (this.SelectedPartInstance.CostReported) {
                            this.UnitCost = this.SelectedPartInstance.UnitCost;
                            this.TotalCost = this.SelectedPartInstance.TotalCost;
                        //} else {
                        //    this.UnitCost = 0;
                        //    this.TotalCost = 0;
                        //}
                    }
                    this._isInitialized = true;
                });
            }
        }

        private async Task ExportOutputHandler(ExportFormat format) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    var path = Path.ChangeExtension(Path.GetTempFileName(), format.ToString().ToLower());
                    using (FileStream file = File.Create(path)) {
                        this.ExportService.Export(file, format);
                    }
                    using(var process=new Process()) {
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.FileName = path;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                    }
                });
            });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            var partInstance = navigationContext.Parameters[ParameterKeys.SelectedPartInstance] as PartInstance;
            if (partInstance is PartInstance) {
                this._isInitialized = false;
                this.SelectedPartInstance = partInstance;

            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) { 

        }
    }
}
