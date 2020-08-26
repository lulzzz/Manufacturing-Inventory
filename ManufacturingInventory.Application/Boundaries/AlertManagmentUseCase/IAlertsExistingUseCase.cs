using System.Collections.Generic;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {
    public interface IAlertsExistingUseCase:IUseCase<AlertUseCaseInput,AlertUseCaseOutput> {
        Task Load();
        Task<IEnumerable<AlertDto>> GetExistingAlert(int userId);
    }
}
