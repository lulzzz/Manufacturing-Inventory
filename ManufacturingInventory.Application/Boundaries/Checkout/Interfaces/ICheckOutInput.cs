using System.Collections.Generic;

namespace ManufacturingInventory.Application.Boundaries.Checkout {
    public interface ICheckOutInput<T> {
        IList<T> Items { get; set; }
    }
}
