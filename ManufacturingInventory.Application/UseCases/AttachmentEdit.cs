using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Domain.IO;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class AttachmentEdit : IAttachmentEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Attachment> _attachmentRepositry;
        private IUnitOfWork _unitOfWork;

        public AttachmentEdit(ManufacturingContext context) {
            this._context = context;
            this._attachmentRepositry = new AttachmentRepository(context);
            this._unitOfWork = new UnitOfWork(context);

        }

        public async Task<AttachmentEditOutput> Execute(AttachmentEditInput input) {
            switch (input.AttachmentOperation) {
                case AttachmentOperation.DELETE:
                    return await this.ExecuteDeleteAttachment(input);
                case AttachmentOperation.NEW:
                    return await this.ExecuteNewAttachment(input);
                default:
                    return new AttachmentEditOutput(null, false, "Invalid Attachment Operation");
            }
        }

        private async Task<AttachmentEditOutput> ExecuteNewAttachment(AttachmentEditInput input) {
            Attachment attachment = new Attachment(DateTime.Now, input.Name, input.SourceReference);
            attachment.Description = input.Description;
            switch (input.AttachmentBy) {
                case GetAttachmentBy.PART:
                    attachment.PartId = input.EntityId;
                    break;        
                case GetAttachmentBy.PARTINSTANCE:
                    attachment.PartInstanceId = input.EntityId;
                    break;
                case GetAttachmentBy.PRICE:
                    attachment.PriceId = input.EntityId;
                    break;
                case GetAttachmentBy.DISTRIBUTOR:
                    attachment.DistributorId = input.EntityId;
                    break;
                case GetAttachmentBy.MANUFACTURER:
                    attachment.ManufacturerId = input.EntityId;
                    break;
                default:
                    return new AttachmentEditOutput(null, false, "Invalid Entity Type");
            }
            var response = await FileService.UploadFileAsync(input.SourceReference, input.Name);
            if (!string.IsNullOrEmpty(response)) {
                attachment.FileReference = response;
                var entity = await this._attachmentRepositry.AddAsync(attachment);
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new AttachmentEditOutput(entity, true, "Success");
                } else {
                    await FileService.DeleteFileAsync(input.SourceReference);
                    await this._unitOfWork.Undo();
                    return new AttachmentEditOutput(null, false, "Save Failed");
                }
            } else {
                return new AttachmentEditOutput(null, false, "Error Uploading File");
            }
        }

        private async Task<AttachmentEditOutput> ExecuteDeleteAttachment(AttachmentEditInput input) {
            var attachment = await this._attachmentRepositry.GetEntityAsync(e => e.Id == input.AttachmentId);
            if (attachment != null) {
                var response = await FileService.DeleteFileAsync(attachment.FileReference);
                if (response) {
                    var entity = await this._attachmentRepositry.DeleteAsync(attachment);
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new AttachmentEditOutput(entity, true, "Success");
                    } else {
                        await FileService.DeleteFileAsync(input.SourceReference);
                        await this._unitOfWork.Undo();
                        return new AttachmentEditOutput(null, false, "Save Failed");
                    }
                } else {
                    return new AttachmentEditOutput(null, false, "Invalid Entity Type");
                }
            } else {
                return new AttachmentEditOutput(null, false, "Attachment Not Found");
            }
        }

        public async Task<IEnumerable<Attachment>> GetAttachments(GetAttachmentBy by, int id) {
            switch (by) {
                case GetAttachmentBy.PART:
                    return await this._attachmentRepositry.GetEntityListAsync(e => e.PartId == id);
                case GetAttachmentBy.PARTINSTANCE:
                    return await this._attachmentRepositry.GetEntityListAsync(e => e.PartInstanceId == id);
                case GetAttachmentBy.PRICE:
                    return await this._attachmentRepositry.GetEntityListAsync(e => e.PriceId == id);
                case GetAttachmentBy.DISTRIBUTOR:
                    return await this._attachmentRepositry.GetEntityListAsync(e => e.DistributorId == id);
                case GetAttachmentBy.MANUFACTURER:
                    return await this._attachmentRepositry.GetEntityListAsync(e => e.ManufacturerId == id);
                default:
                    return null;
            }
        }

        public async Task Load() {
            await this._attachmentRepositry.LoadAsync();
        }

        public async Task<bool> Download(string fileSource,string dest) {
            return await FileService.CopyFileAsync(fileSource, dest);
        }

        public async Task Open(string fileSource) {
            await FileService.OpenFileAsync(fileSource);
        }

        public async Task<Attachment> GetPriceAttachment(int priceId) {
            return await this._attachmentRepositry.GetEntityAsync(e => e.PriceId == priceId);
        }
    }
}
