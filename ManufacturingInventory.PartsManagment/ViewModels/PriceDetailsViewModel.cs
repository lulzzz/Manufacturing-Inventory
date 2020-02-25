using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Common.Application.ValueConverters;
using System.IO;
using System.Collections.Generic;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PriceDetailsViewModel : InventoryViewModelNavigationBase {
        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PriceDetailsMessageService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PriceDetailDispatcher"); }
        public IDialogService FileNameDialog { get { return ServiceContainer.GetService<IDialogService>("FileNameDialog"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }
        public ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>("SaveFileDialog"); } }


        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IPriceEditUseCase _priceEdit;
        private IAttachmentEditUseCase _attachmentEdit;
        private ObservableCollection<Distributor> _distributors;
        private FileNameViewModel _fileNameViewModel;
        private Attachment _priceAttachment;
        

        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string Title { get; set; }
        public bool DialogResult { get; protected set; }
        public string ResultFileName { get; protected set; }
        public string FileBody { get; set; }
        public string DefaultExt { get; set; }
        public string DefaultFileName { get; set; }
        public bool OverwritePrompt { get; set; }

        private Distributor _selectedDistributor;
        private Price _price;
        private double _unitCost;
        private DateTime _timeStamp;
        private DateTime? _validFrom;
        private DateTime? _validUntil;
        private int _minOrder;
        private double _leadTime;

        private PriceEditOption _priceEditOption;
        private bool _canEdit;
        private int? _priceId;
        private int _partId;
        private int _instanceId;
        private bool _isInitialized = false;

        public AsyncCommand InitializeCommand { get; private set; }
        public AsyncCommand SaveCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }

        public AsyncCommand NewAttachmentCommand { get; private set; }
        public AsyncCommand OpenFileCommand { get; private set; }
        public AsyncCommand DownloadFileCommand { get; private set; }
        public AsyncCommand DeleteAttachmentCommand { get; private set; }

        public PriceDetailsViewModel(IPriceEditUseCase priceEditUseCase, IAttachmentEditUseCase attachmentEdit, 
            IEventAggregator eventAggregator,IRegionManager regionManager) {
            this._priceEdit = priceEditUseCase;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;
            this._attachmentEdit = attachmentEdit;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.SaveCommand = new AsyncCommand(this.SaveHandler);
            this.CancelCommand = new AsyncCommand(this.CancelHandler);

            this.NewAttachmentCommand = new AsyncCommand(this.NewAttachmentHandler);
            this.DeleteAttachmentCommand = new AsyncCommand(this.DeleteAttachmentHandler);
            this.DownloadFileCommand = new AsyncCommand(this.DownloadFileHandler);
            this.OpenFileCommand = new AsyncCommand(this.OpenFileHandler);
        }

        public override bool KeepAlive => false;

        public Price Price { 
            get => this._price;
            set => SetProperty(ref this._price, value);
        }

        public ObservableCollection<Distributor> Distributors { 
            get => this._distributors;
            set => SetProperty(ref this._distributors, value);
        }

        public Distributor SelectedDistributor { 
            get => this._selectedDistributor;
            set => SetProperty(ref this._selectedDistributor, value);
        }
        public double UnitCost { 
            get => this._unitCost;
            set => SetProperty(ref this._unitCost, value);
        }

        public DateTime TimeStamp { 
            get => this._timeStamp;
            set => SetProperty(ref this._timeStamp, value);
        }

        public DateTime? ValidFrom { 
            get => this._validFrom;
            set => SetProperty(ref this._validFrom, value);
        }

        public DateTime? ValidUntil { 
            get => this._validUntil;
            set => SetProperty(ref this._validUntil, value);
        }

        public int MinOrder { 
            get => this._minOrder;
            set => SetProperty(ref this._minOrder, value);
        }

        public double LeadTime { 
            get => this._leadTime;
            set => SetProperty(ref this._leadTime, value);
        }

        public bool CanEdit {
            get => this._canEdit;
            set => SetProperty(ref this._canEdit, value);
        }

        public Attachment PriceAttachment { 
            get => this._priceAttachment;
            set => SetProperty(ref this._priceAttachment, value);
        }

        private async Task SaveHandler() {
            PriceEditInput input = new PriceEditInput(this.TimeStamp, this.ValidFrom, this.ValidUntil, true, this.UnitCost, this.MinOrder, this.LeadTime,
                this.SelectedDistributor.Id, this._partId, this._priceEditOption, partInstanceId: this._instanceId, priceId: this._priceId);

            var response = await this._priceEdit.Execute(input);
            if (response.Success) {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                    this._eventAggregator.GetEvent<PriceEditDoneEvent>().Publish();
                    this.CanEdit = false;
                });
            } else {
                this.DispatcherService.BeginInvoke(() => {
                    this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                });
            }
        }

        private async Task CancelHandler() {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.CanEdit = false;
                    this._eventAggregator.GetEvent<PriceEditCancelEvent>().Publish();
                });
            });
        }

        private bool CanExecute() {
            return this._selectedDistributor != null;
        }

        private async Task InitializeHandler() {
            if (!this._isInitialized) {
                var distributors = await this._priceEdit.GetDistributors();
                if (this._priceEditOption!=PriceEditOption.ReplaceWithNew) {
                    var id = (this._priceId.HasValue) ? this._priceId.Value : 0;
                    var price = await this._priceEdit.GetPrice(id);
                    var attachment = await this._attachmentEdit.GetPriceAttachment(id);
                    this.DispatcherService.BeginInvoke(() => {
                        this.Distributors = new ObservableCollection<Distributor>(distributors);
                        this.PriceAttachment = attachment;
                        if (price != null) {
                            this.Price = price;
                            this.UnitCost = price.UnitCost;
                            this.LeadTime = price.LeadTime;
                            this.ValidFrom = price.ValidFrom;
                            this.ValidUntil = price.ValidUntil;
                            this.TimeStamp = price.TimeStamp;
                            this.MinOrder = price.MinOrder;
                            this.SelectedDistributor = this.Distributors.FirstOrDefault(e => e.Id == this.Price.DistributorId);
                        } else {
                            this.UnitCost = 0;
                            this.LeadTime = 0;
                            this.TimeStamp = DateTime.Now;
                            this.MinOrder = 0;
                        }
                    });
                } else {
                    this.DispatcherService.BeginInvoke(() => {
                        this.Distributors = new ObservableCollection<Distributor>(distributors);
                        this.UnitCost = 0;
                        this.LeadTime = 0;
                        this.TimeStamp = DateTime.Now;
                        this.MinOrder = 0;
                    });
                }
                this._isInitialized = true;
            }
        }

        #region AttachmentRegion

        private async Task NewAttachmentHandler() {
            this.OpenFileDialogService.Filter = Constants.FileFilters;
            this.OpenFileDialogService.FilterIndex = this.FilterIndex;
            this.OpenFileDialogService.Title = "Select File To Uplaod";
            if (this.OpenFileDialogService.ShowDialog()) {
                var file = this.OpenFileDialogService.File;
                string tempFileName = file.Name.Substring(0, file.Name.IndexOf("."));
                if (this.ShowAttachmentDialog(tempFileName)) {
                    if (this._fileNameViewModel != null) {
                        var id = (this._priceId.HasValue) ? this._priceId.Value : 0;
                        AttachmentEditInput input = new AttachmentEditInput(this._fileNameViewModel.FileName,
                            this._fileNameViewModel.FileName,
                            this._fileNameViewModel.Description,
                            file.GetFullName(),
                            AttachmentOperation.NEW,
                            Domain.Enums.GetAttachmentBy.PRICE,
                            id);
                        var response = await this._attachmentEdit.Execute(input);
                        if (response.Success) {
                            this.DispatcherService.BeginInvoke(() => {
                                this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                            });
                        } else {
                            this.DispatcherService.BeginInvoke(() => {
                                this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                            });
                        }
                    }
                }
            }
        }

        private async Task DeleteAttachmentHandler() {
            if (this.PriceAttachment != null) {
                string message = "You are about to delete attachment:" + this.PriceAttachment.Name +
                    Environment.NewLine + "Continue?";
                var result = this.MessageBoxService.ShowMessage(message, "Delete", MessageButton.YesNo, MessageIcon.Asterisk);
                if (result == MessageResult.Yes) {
                    AttachmentEditInput input = new AttachmentEditInput(this.PriceAttachment.Id, AttachmentOperation.DELETE);
                    var response = await this._attachmentEdit.Execute(input);
                    if (response.Success) {
                        this.MessageBoxService.ShowMessage(response.Message, "Success", MessageButton.OK, MessageIcon.Information);
                        //await this.Reload();
                    } else {
                        this.MessageBoxService.ShowMessage(response.Message, "Error", MessageButton.OK, MessageIcon.Error);
                        //await this.Reload();
                    }
                }
            }
        }

        private async Task OpenFileHandler() {
            if (this.PriceAttachment != null) {
                await this._attachmentEdit.Open(this.PriceAttachment.FileReference);
            }
        }

        private async Task DownloadFileHandler() {
            if (this.PriceAttachment != null) {
                if (File.Exists(this.PriceAttachment.FileReference)) {
                    string file = this.PriceAttachment.FileReference;
                    string ext = Path.GetExtension(file);
                    this.SaveFileDialogService.DefaultExt = ext;
                    this.SaveFileDialogService.DefaultFileName = Path.GetFileName(file);
                    this.SaveFileDialogService.Filter = this.Filter;
                    this.SaveFileDialogService.FilterIndex = this.FilterIndex;
                    this.DialogResult = SaveFileDialogService.ShowDialog();
                    if (this.DialogResult) {
                        await this._attachmentEdit.Download(file, this.SaveFileDialogService.File.GetFullName());
                    }
                } else {
                    this.MessageBoxService.ShowMessage("File doesn't exist??");
                }
            }
        }

        private bool ShowAttachmentDialog(string currentFile) {
            if (this._fileNameViewModel == null) {
                this._fileNameViewModel = new FileNameViewModel(currentFile);
            }
            this._fileNameViewModel.FileName = currentFile;
            this._fileNameViewModel.Description = "";

            UICommand saveCommand = new UICommand() {
                Caption = "Save Attachment",
                IsCancel = false,
                IsDefault = true,
            };

            UICommand cancelCommand = new UICommand() {
                Id = MessageBoxResult.Cancel,
                Caption = "Cancel",
                IsCancel = true,
                IsDefault = false,
            };
            UICommand result = FileNameDialog.ShowDialog(
            dialogCommands: new List<UICommand>() { saveCommand, cancelCommand },
            title: "New Attachment Dialog",
            viewModel: this._fileNameViewModel);
            return result == saveCommand;

        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext) {
            this._priceId = navigationContext.Parameters[ParameterKeys.PriceId] as int?;
            this._instanceId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.InstanceId]);
            this._partId = Convert.ToInt32(navigationContext.Parameters[ParameterKeys.PartId]);
            var priceEditOption=navigationContext.Parameters[ParameterKeys.PriceEditOption] as object;
            this._priceEditOption=EnumConvertBack.ConvertEnum<PriceEditOption>(priceEditOption);
            this.CanEdit = (this._priceEditOption != PriceEditOption.View);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext) {
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext) {

        }
    }
}
