using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.UseCases {
    public class AlertsExistingUseCase : IAlertsExistingUseCase {

        public Task<AlertUseCaseOutput> Execute(AlertUseCaseInput input) => throw new NotImplementedException();

        public Task<IEnumerable<AlertDto>> GetExistingAlert(int userId) => throw new NotImplementedException();

        public Task Load() => throw new NotImplementedException();
    }
}
