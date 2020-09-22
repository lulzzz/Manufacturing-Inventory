using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.AlertEmailService.Services {
    public class AlertService : IAlertService {
        private readonly ILogger<AlertService> _logger;
        private readonly IEmailer _emailer;
        private ManufacturingContext _context;
        private List<User> _users;

        public AlertService(ILoggerFactory loggerFactory,IEmailer emailer) {
            DbContextOptionsBuilder<ManufacturingContext> optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory;user=aelmendorf;password=Drizzle123!;MultipleActiveResultSets=true");
            this._context = new ManufacturingContext(optionsBuilder.Options);
            this._logger = loggerFactory.CreateLogger<AlertService>();
            this._logger.LogInformation("Alert Service Initialized");
            this._emailer = emailer;
            this._users = new List<User>();

        }

        public async Task Run() {
            await this.GetUsers();
            await this.ProcessAlerts();
        }

        private async Task GetUsers() {
            this._users = await this._context.Users
                .Include(e => e.UserAlerts)
                    .ThenInclude(e => (e.Alert as IndividualAlert).PartInstance.BubblerParameter)
                .Include(e => e.UserAlerts)
                    .ThenInclude(e => (e.Alert as IndividualAlert).PartInstance.Part)
                .Include(e => e.UserAlerts)
                    .ThenInclude(e => (e.Alert as CombinedAlert).StockHolder.PartInstances)
                .Where(e => !String.IsNullOrEmpty(e.Email) && e.UserAlerts.Count() > 0)
                .ToListAsync();
            this._logger.LogInformation("Users Gathered");
        }

        private async Task ProcessAlerts() {
            await foreach (var recipient in this.Process()) {
                await this._emailer.SendMessageAsync(recipient);
            }
        }

        private async IAsyncEnumerable<EmailRecipient> Process() {
            foreach (var user in this._users) {
                EmailRecipient recipient = new EmailRecipient(user.Email);
                recipient.Alerts = user.UserAlerts
                    .Where(e => e.IsEnabled)
                    .Select(userAlert => new AlertDto(userAlert.Alert))
                    .OrderBy(e => e.AlertStatus)
                    .ToList();

                await recipient.BuildAlertTable();
                recipient.FinalizeMessage();
                yield return recipient;
            }
        }
    }
}
