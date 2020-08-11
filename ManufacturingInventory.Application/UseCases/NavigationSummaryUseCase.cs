using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;

namespace ManufacturingInventory.Application.UseCases {
    public class NavigationSummaryUseCase : INavigationSummaryUseCase {
        private ManufacturingContext _context;
        private IRepository<MonthlySummary> _reportRepository;
        private IUnitOfWork _unitOfWork;

        public NavigationSummaryUseCase(ManufacturingContext context) {
            this._reportRepository = new MonthlySummaryRepository(context);
            this._unitOfWork = new UnitOfWork(context);
            this._context = context;
        }

        public async Task<NavigationSummaryOutput> Execute(NavigationSummaryInput input) {
            if (input.Action == Boundaries.EditAction.Delete) {
                var deleted=await this._reportRepository.DeleteAsync(input.Summary);
                if (deleted != null) {
                    var count = await this._unitOfWork.Save();
                    if (count > 0) {
                        return new NavigationSummaryOutput(deleted, true, "Summary Deleted");
                    } else {
                        await this._unitOfWork.Undo();
                        return new NavigationSummaryOutput(null, false, "Error: Failed to Delete Summary");
                    }
                } else {
                    return new NavigationSummaryOutput(null, false, "Error: Failed to Delete Summary");
                }
            } else {
                return new NavigationSummaryOutput(null, false, "Error: Only Delete Action Is Available");
            }
        }
       
        public async Task<IEnumerable<MonthlySummary>> GetExistingReports() {
            return await this._reportRepository.GetEntityListAsync();
        }

        public async Task Load() {
            await this._reportRepository.LoadAsync();
        }
    }
}
