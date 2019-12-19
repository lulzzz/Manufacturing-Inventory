using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.Common.Model;
using System.Threading.Tasks;

namespace ManufacturingInventory.Common.Services.CheckInOutServices.Concrete {
    public class CheckOutService : ICheckOutService {

        public readonly ManufacturingContext _context;

        public CheckOutService(ManufacturingContext context) {
            this._context = context;
        }

        public Transaction CheckOutBubbler(PartInstance instance, Location location, double measured) {

            return null;
        }

        public async Task<Transaction> CheckOutBubblerAsync(PartInstance instance, Location location, double measured) {

            return null;
        }

        public Transaction CheckOutStandard(PartInstance instance, Location location, int quantity) {
            return null;
        }

        public async Task<Transaction> CheckOutStandardAsync(PartInstance instance, Location location, int quantity) {

            return null;
        }
    }
}
