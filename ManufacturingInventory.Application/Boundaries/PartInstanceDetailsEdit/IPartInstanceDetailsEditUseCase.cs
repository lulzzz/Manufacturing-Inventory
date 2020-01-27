using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit {
    public interface IPartInstanceDetailsEditUseCase: IUseCase<PartInstanceDetailsEditInput, PartInstanceDetailsEditOutput> {
        Task<IEnumerable<Location>> GetLocations();
        Task<IEnumerable<Attachment>> GetAttachments(int instanceId);
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<Transaction>> GetTransactions(int instanceId);
    }
}
