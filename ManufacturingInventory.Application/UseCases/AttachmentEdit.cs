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
                case AttachmentOperation.OPEN:
                    return await this.ExecuteNewAttachment(input);
                case AttachmentOperation.DOWNLOAD:
                    return null;
                case AttachmentOperation.RENAME:
                    return null;
                case AttachmentOperation.DELETE:
                    return null;
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
                    attachment.PartId = input.EntityId;
                    break;
                case GetAttachmentBy.PRICE:
                    attachment.PartId = input.EntityId;
                    break;
                case GetAttachmentBy.DISTRIBUTOR:
                    attachment.PartId = input.EntityId;
                    break;
                case GetAttachmentBy.MANUFACTURER:
                    attachment.PartId = input.EntityId;
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
                return new AttachmentEditOutput(null, false, "Invalid Entity Type");
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

        public Task Download(int attachmentId) => throw new NotImplementedException();

        public Task Open(int attachmentId) => throw new NotImplementedException();

    }
}
