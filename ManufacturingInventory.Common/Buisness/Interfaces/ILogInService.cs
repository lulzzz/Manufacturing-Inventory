using ManufacturingInventory.Common.Buisness.Concrete;

namespace ManufacturingInventory.Common.Buisness.Interfaces {
    public interface ILogInService {
        LogInResponce LogInWithPassword(string username, string password, bool storePassword);
    }
}