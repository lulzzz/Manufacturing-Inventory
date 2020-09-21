using ManufacturingInventory.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingInventory.AlertEmailService.Services {
    public interface IEmailRecipient {
        List<AlertDto> Alerts { get; set; }
        string EmailAddress { get; set; }
        bool IsFinalized { get; }
        string Message { get; }
        MessageBuilder MessageBuilder { get; }

        Task BuildAlertTable();
        void FinalizeMessage();
    }
}