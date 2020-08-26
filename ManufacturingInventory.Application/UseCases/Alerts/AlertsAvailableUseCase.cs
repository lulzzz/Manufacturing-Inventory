using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.AlertManagmentUseCase;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.Application.UseCases {
    public class AlertsAvailableUseCase : IAlertsAvailableUseCase {
        private ManufacturingContext _context;
        private IRepository<Alert> _alertRepository;
        private IRepository<User> _userRepository;
        private IEntityProvider<PartInstance> _partInstanceProvider;


        public AlertsAvailableUseCase(ManufacturingContext context) {
            this._context = context;
            this._alertRepository = new AlertRepository(context);
            this._userRepository = new UserRepository(context);
            this._partInstanceProvider = new PartInstanceProvider(context);
        }
        
        public async Task<AlertUseCaseOutput> Execute(AlertUseCaseInput input) {
            return null;
        }

        public async Task<IEnumerable<AlertDto>> GetAvailableAlerts(int userId) {
            var tempIndividual = this._context.UserAlerts.Include(e => e.Alert).Where(e => e.Alert.AlertType == AlertType.IndividualAlert && e.UserId == 1).Select(e => e.Alert);
            var tempCombined = this._context.UserAlerts.Include(e => e.Alert).Where(e => e.Alert.AlertType == AlertType.CombinedAlert && e.UserId == 1).Select(e => e.Alert);
            List<AlertDto> alerts = new List<AlertDto>();

            await Task.Run(() => {
                foreach (var temp in tempIndividual) {
                    var alert = this._context.Alerts.OfType<IndividualAlert>().Include(e => e.PartInstance).ThenInclude(e => e.Part).FirstOrDefault(e => e.Id == temp.Id);
                    if (alert != null) {
                        alerts.Add(new AlertDto(temp));
                    }
                }


                foreach (var temp in tempCombined) {
                    var alert = this._context.Alerts.OfType<CombinedAlert>().Include(e => e.StockHolder).ThenInclude(e => e.PartInstances).ThenInclude(e => e.Part).FirstOrDefault(e => e.Id == temp.Id);
                    if (alert != null) {
                        alerts.Add(new AlertDto(alert));
                    }
                }
            });
            return alerts;
        }

        public Task Load() => throw new NotImplementedException();
    }
}
