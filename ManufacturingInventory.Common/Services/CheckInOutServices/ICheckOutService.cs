using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Common.Services {
    public interface ICheckOutService {
        Transaction CheckOutBubbler(PartInstance instance,Location location,double measured);
        Transaction CheckOutStandard(PartInstance instance, Location location, int quantity);
        Task<Transaction> CheckOutBubblerAsync(PartInstance instance, Location location, double measured);
        Task<Transaction> CheckOutStandardAsync(PartInstance instance, Location location, int quantity);
    }
}
