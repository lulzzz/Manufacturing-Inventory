using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.DbContextExtensions;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.Application.UseCases {
    public class AlertsAvailableUseCase : IAlertsAvailableUseCase {
        private ManufacturingContext _context;
        private IRepository<UserAlert> _userAlertRepository;
        private IUnitOfWork _unitOfWork;

        public AlertsAvailableUseCase(ManufacturingContext context) {
            this._context = context;
            this._userAlertRepository = new UserAlertRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }
        
        public async Task<AlertUseCaseOutput> Execute(AlertUseCaseInput input) {
            switch (input.AlertAction) {
                case AlertAction.Subscribe: {
                    UserAlert userAlert = new UserAlert();
                    userAlert.AlertId = input.AlertDto.AlertId;
                    userAlert.UserId = input.UserId; 
                    userAlert.IsEnabled = true;
                    var added = await this._userAlertRepository.AddAsync(userAlert);
                    if (added != null) {
                        var count = await this._unitOfWork.Save();
                        if (count > 0) {
                            return new AlertUseCaseOutput(null, true, "Success, Reloading...");
                        } else {
                            await this._unitOfWork.Undo();
                            return new AlertUseCaseOutput(null, false, "Error: UserAlert Save Failed");
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        return new AlertUseCaseOutput(null, false, "Error: Could not add new UserAlert");
                    }
                }
                case AlertAction.UnSubscribe: {
                    return new AlertUseCaseOutput(null,false,"Error: Action Not available");
                }
                case AlertAction.ToggleEnable: {
                    return new AlertUseCaseOutput(null, false, "Error: Action Not available");
                }
                default:
                    return new AlertUseCaseOutput(null,false,"Error: Invalid option, please contact admin");
            }
        }

        public async Task<IEnumerable<AlertDto>> GetAvailableAlerts(int userId) {
            return await this._context.UserAlerts
                .Include(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                .Include(e => (e.Alert as IndividualAlert).PartInstance.Part)
                .Include(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                .Where(e => e.UserId !=userId).Select(e => new AlertDto(e.Alert)).ToListAsync();
        }

        public async Task Load() {
            await this._userAlertRepository.LoadAsync();
        }
    }
}
