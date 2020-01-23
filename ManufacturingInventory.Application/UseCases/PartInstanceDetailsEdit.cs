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
//using Prism.Ioc;
//using Prism.DryIoc;
//using DryIoc;

namespace ManufacturingInventory.Application.UseCases {
    public class PartInstanceDetailsEdit : IPartInstanceDetailsEditUseCase {
        private IRepository<PartInstance> _instanceRepository;
        private IRepository<Attachment> _attachmentRepository;
        private IEntityProvider<Transaction> _transactionProvider;
        private IEntityProvider<Category> _categoryProvider;
        private IEntityProvider<Location> _locationProvider;
        private IRepository<BubblerParameter> _bubblerRepository;
        private IUnitOfWork _unitOfWork;
        private ManufacturingContext _context;

        public PartInstanceDetailsEdit(ManufacturingContext context) {
            this._context = context;
            this._instanceRepository = new PartInstanceRepository(context);
            this._attachmentRepository = new AttachmentRepository(context);
            this._transactionProvider = new TransactionProvider(context);
            this._categoryProvider = new CategoryProvider(context);
            this._locationProvider = new LocationProvider(context);
            this._bubblerRepository =new BubblerParameterRepository(context);
            this._unitOfWork = new UnitOfWork(context);
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
