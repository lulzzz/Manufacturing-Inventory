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
    public class AlertsExistingUseCase : IAlertsExistingUseCase {
        private ManufacturingContext _context;
        private IRepository<UserAlert> _userAlertRepository;
        private IUnitOfWork _unitOfWork;

        public AlertsExistingUseCase(ManufacturingContext context) {
            this._context = context;
            this._userAlertRepository = new UserAlertRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<AlertUseCaseOutput> Execute(AlertUseCaseInput input) {
            switch (input.AlertAction) {
                case AlertAction.Subscribe: {
                    return new AlertUseCaseOutput(null, false, "Error: Action Not available");
                }
                case AlertAction.UnSubscribe: {
                    var userAlert = await this._userAlertRepository.GetEntityAsync(e => e.UserId == input.UserId && e.AlertId == input.AlertDto.AlertId);
                    if (userAlert != null) {
                        var deleted = this._context.Remove(userAlert).Entity;
                        if (deleted != null) {
                            var count = await this._unitOfWork.Save();
                            if (count > 0) {
                                return new AlertUseCaseOutput(null, true, "Success: Reloading...");
                            } else {
                                await this._unitOfWork.Undo();
                                return new AlertUseCaseOutput(null, false, "Error: UnSubscribe Failed");
                            }
                        } else {
                            return new AlertUseCaseOutput(null, false, "Error: Could not find UserAlert");
                        }
                    } else {
                        return new AlertUseCaseOutput(null, false, "Internal Error: Could not find UserAlert");
                    }

                }
                case AlertAction.ToggleEnable: {
                    var userAlert = await this._userAlertRepository.GetEntityAsync(e => e.UserId == input.UserId && e.AlertId == input.AlertDto.AlertId);
                    if (userAlert != null) {
                        userAlert.IsEnabled = !userAlert.IsEnabled;
                        var updated = await this._userAlertRepository.UpdateAsync(userAlert);
                        if (updated != null) {
                            var count = await this._unitOfWork.Save();
                            if (count > 0) {
                                string message = (updated.IsEnabled) ? "Success: Enabled" : "Success: Disabled";
                                return new AlertUseCaseOutput(null, true, message+" Reloading...");
                            } else {
                                await this._unitOfWork.Undo();
                                return new AlertUseCaseOutput(null, false, "Error: UserAlert Save Failed");
                            }
                        } else {
                            await this._unitOfWork.Undo();
                            return new AlertUseCaseOutput(null, false, "Error: UserAlert Save Failed");
                        }
                    } else {
                        await this._unitOfWork.Undo();
                        return new AlertUseCaseOutput(null, false, "Error: Could not find UserAlert");
                    }
                }
                default:
                    return new AlertUseCaseOutput(null, false, "Error: Invalid option, please contact admin");
            }
        }

        public async Task<IEnumerable<AlertDto>> GetExistingAlerts(int userId) {
            return (await this._userAlertRepository.GetEntityListAsync(e => e.UserId == userId)).Select(userAlert=>new AlertDto(userAlert));
        }

        public async Task Load() {
            await this._userAlertRepository.LoadAsync();
        }
    }
}
