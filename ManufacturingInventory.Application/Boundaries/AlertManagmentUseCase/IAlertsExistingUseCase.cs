using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Domain.Enums;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {
    public interface IAlertsExistingUseCase:IUseCase<AlertsExisitingInput,AlertsExisitingOutput> {
        Task Load();
        Task<IEnumerable<UserAlert>> GetExistingAlert(int userId);
    }
}
