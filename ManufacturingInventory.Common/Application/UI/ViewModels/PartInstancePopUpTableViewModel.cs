using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class PartInstancePopUpTableViewModel {
        public PartInstancePopUpTableViewModel() {
            this.PartInstances = new ObservableCollection<PartInstance>();
            this.SelectedPartInstances = new ObservableCollection<PartInstance>();
        }

        public PartInstancePopUpTableViewModel(IEnumerable<PartInstance> partInstances) {
            this.PartInstances = new ObservableCollection<PartInstance>(partInstances);
            this.SelectedPartInstances = new ObservableCollection<PartInstance>();
        }

        public virtual ObservableCollection<PartInstance> PartInstances { get; set; }
        public virtual ObservableCollection<PartInstance> SelectedPartInstances { get; set; }

    }
}
