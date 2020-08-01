using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public interface IMonthlySummaryUseCase:IUseCase<MonthlySummaryInput,MonthlySummaryOutput> {
        Task<bool> SaveCurrentSnapshot();
        Task Load();
    }

    public interface ICurrentInventoryUseCase : IUseCase<CurrentInventoryInput, CurrentInventoryOutput> {
        Task Load();
    }

    public interface ITransactionSummaryUseCase : IUseCase<TransactionSummaryInput, TransactionSummaryOutput> {
        Task Load();
    }
}
