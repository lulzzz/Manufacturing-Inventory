using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces {

    public interface IAttachmentEditUseCase<T, P> : IUseCase<T, P> {
        Task<IEnumerable<Attachment>> GetAttachments(int id);
    }

    public interface IAttachmentPartEditUseCase: IAttachmentEditUseCase<AttachmentPartEditInput, AttachmentPartEditOutput> {

    }

    public interface IAttachmentPriceEditUseCase : IAttachmentEditUseCase<AttachmentPriceEditInput, AttachmentPriceEditOutput> {
    
    }

    public interface IAttachmentDistributorEditUseCase : IAttachmentEditUseCase<AttachmentDistributorEditInput, AttachmentDistributorEditOutput> {
    
    }

    public interface IAttachmentManufacturerEditUseCase : IAttachmentEditUseCase<AttachmentManufacturerEditInput, AttachmentManufacturerEditOutput> {
    
    }

    public interface IAttachmentPartInstanceEditUseCase : IAttachmentEditUseCase<AttachmentPartInstanceEditInput, AttachmentPartInstanceEditOutput> {
    
    }
}
