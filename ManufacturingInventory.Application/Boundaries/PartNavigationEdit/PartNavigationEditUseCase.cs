using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.PartNavigationEdit {
    public interface IPartNavigationEditUseCase:IUseCase<PartNavigationEditInput,PartNavigationEditOutput> {
        Task<Part> GetPart(int id);
        Task<IEnumerable<Part>> GetPartsAsync();
        IEnumerable<Part> GetParts();
        Task LoadAsync();
        void Load();
    }
}
