using System.Threading.Tasks;

namespace ManufacturingInventory.Infrastructure.Model {
    public interface IUnitOfWork {
        Task<int> Save();
        Task Undo();
    }

    public interface IUnitOfWorkV2 {
        Task<int> SaveAsync();
        Task UndoAsync();
        int Save();
        void Undo();
    }
}
