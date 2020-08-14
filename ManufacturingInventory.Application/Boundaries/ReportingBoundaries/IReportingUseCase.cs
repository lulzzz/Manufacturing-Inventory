using System.Collections.Generic;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public interface IMonthlyReportUseCase:IUseCase<MonthlyReportInput,MonthlyReportOutput> {
        Task Load();
    }

    public interface ICurrentInventoryUseCase : IUseCase<CurrentInventoryInput, CurrentInventoryOutput> {
        Task Load();
    }

    public interface ITransactionLogUseCase : IUseCase<TransactionSummaryInput, TransactionSummaryOutput> {
        Task Load();
    }
}
