using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Domain.Enums;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit {



    public interface IAttachmentEditUseCase:IUseCase<AttachmentEditInput,AttachmentEditOutput> {
        Task<IEnumerable<Attachment>> GetAttachments(GetAttachmentBy by, int id);
        Task Load();
    }
}
