using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model {
    public interface IUnitOfWork {
        Task<int> Save();
        Task Undo();
    }
}
