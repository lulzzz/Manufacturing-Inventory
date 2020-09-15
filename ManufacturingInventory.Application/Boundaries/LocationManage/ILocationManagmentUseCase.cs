using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.LocationManage {
    public interface ILocationManagmentUseCase:IUseCase<LocationManagmentInput,LocationManagmentOutput> {
        Task Load();
        Task<IEnumerable<LocationDto>> GetLocations();
        Task<LocationDto> GetLocation(int locationId);
        Task<IEnumerable<InstanceDto>> GetLocationInstances(int locationId);
        Task<IEnumerable<PartDto>> GetLocationParts(int locationId);
    }
}
