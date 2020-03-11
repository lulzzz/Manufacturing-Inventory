using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class NewDistributorViewModel {

        public NewDistributorViewModel() {

        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
