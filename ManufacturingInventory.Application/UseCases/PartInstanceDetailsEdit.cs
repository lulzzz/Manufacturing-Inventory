using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Services;

namespace ManufacturingInventory.Application.UseCases {
    public class PartInstanceDetailsEdit : IPartInstanceDetailsEditUseCase {
        private IRepository<Attachment> _attachmentRepository;
        private IEntityProvider<Transaction> _transactionProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IRepository<Location> _locationRepository;
        private IUnitOfWork _unitOfWork;

        public PartInstanceDetailsEdit(IRepository<Attachment> attachmentRepository, 
            IEntityProvider<Transaction> transactionProvider, 
            IEntityProvider<Category> categoryProvider, 
            IRepository<Location> locationRepository,
            IUnitOfWork unitOfWork) {

            this._attachmentRepository = attachmentRepository;
            this._transactionProvider = transactionProvider;
            this._categoryProvider = categoryProvider;
            this._locationRepository = locationRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Attachment>> GetAttachments() {
            return await this._attachmentRepository.GetEntityListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories() {
            return await this._categoryProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocations() {
            return await this._locationRepository.GetEntityListAsync();
        }

        public Task<PartInstanceDetailsEditOutput> Execute(PartInstanceDetailsEditInput input) {
            //Needs Implementation
            return null;
        }


    }
}
