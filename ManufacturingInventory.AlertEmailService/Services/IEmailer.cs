using System.Threading.Tasks;

namespace ManufacturingInventory.AlertEmailService.Services {
    public interface IEmailer {
        Task SendMessageAsync(EmailRecipient emailRecipient);
    }
}