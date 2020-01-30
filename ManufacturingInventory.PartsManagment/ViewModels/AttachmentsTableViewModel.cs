using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using DevExpress.Mvvm;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Views;
using Prism.Regions;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using PrismCommands = Prism.Commands;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Prism.Events;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Domain.Enums;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class AttachmentsTableViewModel : InventoryViewModelBase {
        private ObservableCollection<Attachment> _attachments;
        private int _entityId;
        private GetAttachmentBy _getBy;
        private Attachment _selectedAttachment;
        private FileNameViewModel _fileNameViewModel;

        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IAttachmentEditUseCase _attachmentEdit;

        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string Title { get; set; }
        public bool DialogResult { get; protected set; }
        public string ResultFileName { get; protected set; }
        public string FileBody { get; set; }
        public string DefaultExt { get; set; }
        public string DefaultFileName { get; set; }
        public bool OverwritePrompt { get; set; }

        public IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("AttachmentNotifications"); } }
        public IDispatcherService DispatcherService { get { return ServiceContainer.GetService<IDispatcherService>("AttachmentDispatcher"); } }
        public IDialogService FileNameDialog { get { return ServiceContainer.GetService<IDialogService>("FileNameDialog"); } }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }
        public ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>("SaveFileDialog"); } }

        public AsyncCommand NewAttachmentCommand { get; private set; }
        public AsyncCommand OpenFileCommand { get; private set; }
        public AsyncCommand DownloadFileCommand { get; private set; }
        public AsyncCommand DeleteAttachmentCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }

        public AttachmentsTableViewModel(IAttachmentEditUseCase attachmentEdit, IEventAggregator eventAggregator, IRegionManager regionManager) {
            this._attachmentEdit = attachmentEdit;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this.InitializeCommand = new AsyncCommand(this.InitializeHandler);
            this.NewAttachmentCommand = new AsyncCommand(this.NewAttachmentHandler);
        }

        public override bool KeepAlive => false;

        public ObservableCollection<Attachment> Attachments { 
            get => this._attachments; 
            set => SetProperty(ref this._attachments,value);
        }

        public int SelectedEntityId {
            get => this._entityId;
            set => this._entityId=value;
        }

        public Attachment SelectedAttachment { 
            get => this._selectedAttachment;
            set => SetProperty(ref this._selectedAttachment, value);
        }

        public GetAttachmentBy GetBy { 
            get => this._getBy; 
            set => this._getBy = value;
        }

        private async Task NewAttachmentHandler() {
            this.OpenFileDialogService.Filter = Constants.FileFilters;
            this.OpenFileDialogService.FilterIndex = this.FilterIndex;
            this.OpenFileDialogService.Title = "Select File To Uplaod";
            if (this.OpenFileDialogService.ShowDialog()) {
                var file = this.OpenFileDialogService.File;
                string tempFileName = file.Name.Substring(0, file.Name.IndexOf("."));
                if (this.ShowAttachmentDialog(tempFileName)) {
                    if (this._fileNameViewModel != null) {
                        AttachmentEditInput input = new AttachmentEditInput(this._fileNameViewModel.FileName,
                            this._fileNameViewModel.FileName,
                            this._fileNameViewModel.Description,
                            file.GetFullName(),
                            AttachmentOperation.NEW,
                            this.GetBy,
                            this.SelectedEntityId);
                        await this._attachmentEdit.Execute(input);
                    }
                }
            }
        }

        private void DeleteAttachmentHandler() {
            if (this.SelectedAttachment != null) {
                string message = "You are about to delete attachment:" + this.SelectedAttachment.Name +
                    Environment.NewLine + "Continue?";
                var result = this.MessageBoxService.ShowMessage(message, "Delete", MessageButton.YesNo, MessageIcon.Asterisk);
                if (result == MessageResult.Yes) {

                    if (File.Exists(this.SelectedAttachment.FileReference)) {
                        var success = true;
                        try {
                            File.Delete(this.SelectedAttachment.FileReference);
                        } catch {
                            this.MessageBoxService.ShowMessage("Failed to Delete Attachment", "Error", MessageButton.OK, MessageIcon.Error);
                            success = false;
                        }
                        if (success) {
                            //var responce = this._dataManager.DeleteProductAttachment(this.SelectedAttachment);
                            //if (responce.Success) {
                            //    this.MessageBoxService.ShowMessage(responce.Message, "Success", MessageButton.OK, MessageIcon.Information);
                            //} else {
                            //    this.MessageBoxService.ShowMessage("", "Error", MessageButton.OK, MessageIcon.Error);
                            //}
                        }
                    }
                    //this.ReloadAsync();
                }
            }
        }

        private void OpenFileHandler() {
            if (this.SelectedAttachment != null) {
                if (File.Exists(this.SelectedAttachment.FileReference)) {
                    Process.Start(this.SelectedAttachment.FileReference);
                }
            }
        }

        private void DownloadFileHandler() {
            if (this.SelectedAttachment != null) {
                if (File.Exists(this.SelectedAttachment.FileReference)) {
                    string file = this.SelectedAttachment.FileReference;
                    string ext = Path.GetExtension(file);
                    this.SaveFileDialogService.DefaultExt = ext;
                    this.SaveFileDialogService.DefaultFileName = Path.GetFileName(file);
                    this.SaveFileDialogService.Filter = this.Filter;
                    this.SaveFileDialogService.FilterIndex = this.FilterIndex;
                    this.DialogResult = SaveFileDialogService.ShowDialog();
                    if (this.DialogResult) {
                        File.Copy(file, this.SaveFileDialogService.File.GetFullName());
                    }
                } else {
                    this.MessageBoxService.ShowMessage("File doesn't exist??");
                }
            } else {
                this.MessageBoxService.ShowMessage("Selection is null??");
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
    
        private async Task InitializeHandler() {
            var attachments =await this._attachmentEdit.GetAttachments(this.GetBy,this.SelectedEntityId);
            this.Attachments = new ObservableCollection<Attachment>(attachments);
        }
    }
}
