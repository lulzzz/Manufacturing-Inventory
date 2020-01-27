using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.Checkout {
    public interface ICheckOutBubblerUseCase:IUseCase<CheckOutBubblerInput, CheckOutOutput> {
        Task<IEnumerable<Consumer>> GetConsumers();
        Task<IEnumerable<Condition>> GetConditions();
    }
}
