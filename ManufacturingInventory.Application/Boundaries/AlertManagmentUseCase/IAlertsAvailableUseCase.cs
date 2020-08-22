using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Domain.Enums;

namespace ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase {
    public interface IAlertsAvailableUseCase:IUseCase<AlertsAvailableInput,AlertsAvailableOutput> {
        Task Load();
        Task<IEnumerable<Alert>> GetAvailableAlerts(int userId);    
    }
}
