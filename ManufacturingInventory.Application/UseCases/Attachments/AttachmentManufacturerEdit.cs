using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {

    public class AttachmentManufacturerEdit : IAttachmentManufacturerEditUseCase {
        private IRepository<Attachment> _repository;
        private IUnitOfWork _unitOfWork;
        private IFileService _fileService;

        public AttachmentManufacturerEdit(IRepository<Attachment> repository, IUnitOfWork unitOfWork, IFileService fileService) {
            this._repository = repository;
            this._unitOfWork = unitOfWork;
            this._fileService = fileService;
        }

        public Task<AttachmentManufacturerEditOutput> Execute(AttachmentManufacturerEditInput input) {
            return null;
        }

        public async Task<IEnumerable<Attachment>> GetAttachments(int id) {
            return await this._repository.GetEntityListAsync(e => e.Id == id);
        }
    }
}
