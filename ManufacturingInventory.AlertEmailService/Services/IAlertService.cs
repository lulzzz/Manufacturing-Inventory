using System.Threading.Tasks;

namespace ManufacturingInventory.AlertEmailService.Services {
    public interface IAlertService {
        Task Run();
    }
}