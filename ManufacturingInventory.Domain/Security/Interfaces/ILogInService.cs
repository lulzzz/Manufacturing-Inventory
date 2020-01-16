using ManufacturingInventory.Domain.Buisness.Concrete;

namespace ManufacturingInventory.Domain.Buisness.Interfaces {
    public interface ILogInService {
        LogInResponce LogInWithPassword(string username, string password, bool storePassword);
    }
}