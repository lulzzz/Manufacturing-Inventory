using ManufacturingInventory.Domain.Buisness.Concrete;

namespace ManufacturingInventory.Domain.Security.Interfaces {
    public interface ILogInService {
        LogInResponce LogInWithPassword(string username, string password, bool storePassword);
    }
}