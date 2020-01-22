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
        private IRepository<PartInstance> _instanceRepository;
        private IRepository<Attachment> _attachmentRepository;
        private IEntityProvider<Transaction> _transactionProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IEntityProvider<Location> _locationProvider;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IUnitOfWork _unitOfWork;

        public PartInstanceDetailsEdit(IRepository<PartInstance> instanceRepository,
            IRepository<Attachment> attachmentRepository, 
            IEntityProvider<Transaction> transactionProvider, 
            IEntityProvider<Category> categoryProvider,
            IEntityProvider<Location> locationProvider,
            IRepository<BubblerParameter> bubblerRepository,
            IUnitOfWork unitOfWork) {
            this._instanceRepository = instanceRepository;
            this._attachmentRepository = attachmentRepository;
            this._transactionProvider = transactionProvider;
            this._categoryProvider = categoryProvider;
            this._locationProvider = locationProvider;
            this._bubblerRepository = bubblerRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Attachment>> GetAttachments() {
            return await this._attachmentRepository.GetEntityListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories() {
            return await this._categoryProvider.GetEntityListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocations() {
            return await this._locationProvider.GetEntityListAsync();
        }

        public async Task<PartInstanceDetailsEditOutput> Execute(PartInstanceDetailsEditInput input) {
            if (input.IsNew) {
                var entity = await this._instanceRepository.AddAsync(input.PartInstance);
                if (entity != null) {
                    return new PartInstanceDetailsEditOutput(entity, true,entity.Name+" Added");
                } else {
                    return new PartInstanceDetailsEditOutput(null, false, "Failed");
                }
            } else {
                if (input.PartInstance.IsBubbler) {
                    var bubbler = await this._bubblerRepository.UpdateAsync(input.PartInstance.BubblerParameter);
                    var entity = await this._instanceRepository.UpdateAsync(input.PartInstance);
                    if (entity != null && bubbler!=null) {
                        var count=await this._unitOfWork.Save();
                        return new PartInstanceDetailsEditOutput(entity, true, entity.Name + " Updated Count:"+count);
                    } else {
                        return new PartInstanceDetailsEditOutput(null, false, "Failed");
                    }
                } else {//not bubbler
                    var entity = await this._instanceRepository.UpdateAsync(input.PartInstance);
                    if (entity != null) {
                        var count = await this._unitOfWork.Save();
                        return new PartInstanceDetailsEditOutput(entity, true, entity.Name + " Updated Count:" + count);
                    } else {
                        return new PartInstanceDetailsEditOutput(null, false, "Failed");
                    }
                }
            }

        }
    }
}
