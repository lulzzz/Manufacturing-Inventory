using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public class CurrentInventoryUseCase : ICurrentInventoryUseCase {
        private ManufacturingContext _context;
        private IEntityProvider<PartInstance> _partInstanceProvider;


        public CurrentInventoryUseCase(ManufacturingContext context) {
            this._context = context;
            this._partInstanceProvider = new PartInstanceProvider(context);
        }

        public async Task<CurrentInventoryOutput> Execute(CurrentInventoryInput input) {
            IEnumerable<PartInstance> parts = new List<PartInstance>();
            switch (input.CollectType) {
                case CollectType.OnlyCostReported:
                    parts = await this._partInstanceProvider.GetEntityListAsync(part => part.CostReported);
                    break;
                case CollectType.AllItems:
                    parts = await this._partInstanceProvider.GetEntityListAsync();
                    break;
                case CollectType.OnlyCostNotReported:
                    parts = await this._partInstanceProvider.GetEntityListAsync(part => !part.CostReported);
                    break;
            }
            List<CurrentInventoryItem> items = new List<CurrentInventoryItem>();
            foreach (var part in parts) {
                if (part.IsBubbler) {
                    var dateIn = part.Transactions.OrderByDescending(e => e.TimeStamp).FirstOrDefault(e => e.InventoryAction == InventoryAction.INCOMING);
                    items.Add(new CurrentInventoryItem() { Id = part.Id, PartCategory = part.Part.Name, Part = part.Name, Quantity = part.BubblerParameter.NetWeight, Cost = part.TotalCost });
                } else {
                    items.Add(new CurrentInventoryItem() { Id = part.Id, PartCategory = part.Part.Name, Part = part.Name, Quantity = part.Quantity, Cost = part.TotalCost });
                }
            }
            return new CurrentInventoryOutput(items, true, "Success");
        }

        public async Task Load() {
            await this._partInstanceProvider.LoadAsync();
        }
    }
}
