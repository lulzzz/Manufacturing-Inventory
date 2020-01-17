using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces;
using ManufacturingInventory.Domain.Buisness.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class AttachmentDistributorEdit : IAttachmentDistributorEditUseCase {
        private IRepository<Attachment> _repository;
        private IUnitOfWork _unitOfWork;
        private IFileService _fileService;

        public AttachmentDistributorEdit(IRepository<Attachment> repository, IUnitOfWork unitOfWork, IFileService fileService) {
            this._repository = repository;
            this._unitOfWork = unitOfWork;
            this._fileService = fileService;
        }

        public Task<AttachmentDistributorEditOutput> Execute(AttachmentDistributorEditInput input) {
            return null;
        }

        public async Task<IEnumerable<Attachment>> GetAttachments(int id) {
            return await this._repository.GetEntityListAsync(e => e.Id == id);
        }
    }
}
