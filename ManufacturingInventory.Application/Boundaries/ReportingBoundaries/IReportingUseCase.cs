using System.Collections.Generic;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Interfaces;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {

    public interface IMonthlySummaryUseCase:IUseCase<MonthlySummaryInput,MonthlySummaryOutput> {
        Task<MonthlySummary> SaveMonthlySummary(MonthlySummary monthlySummary);
        Task<IEnumerable<string>> GetExistingReports();
        Task<MonthlySummary> LoadExisitingReport(string month);
        Task Load();
    }

    public interface INavigationSummaryUseCase : IUseCase<NavigationSummaryInput, NavigationSummaryOutput> {
        Task<IEnumerable<MonthlySummary>> GetExistingReports();
        Task Load();
    }

    public interface ICurrentInventoryUseCase : IUseCase<CurrentInventoryInput, CurrentInventoryOutput> {
        Task Load();
    }

    public interface ITransactionSummaryUseCase : IUseCase<TransactionSummaryInput, TransactionSummaryOutput> {
        Task Load();
    }
}
