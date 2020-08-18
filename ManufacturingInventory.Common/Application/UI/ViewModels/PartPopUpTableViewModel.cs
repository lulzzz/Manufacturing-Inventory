using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class PartPopUpTableViewModel {
        public PartPopUpTableViewModel() {
            this.Parts = new ObservableCollection<Part>();
            this.SelectedParts = new ObservableCollection<Part>();
        }

        public PartPopUpTableViewModel(IEnumerable<Part> parts) {
            this.Parts = new ObservableCollection<Part>(parts);
            this.SelectedParts = new ObservableCollection<Part>();
        }

        public virtual ObservableCollection<Part> Parts { get; set; }
        public virtual ObservableCollection<Part> SelectedParts { get; set; }

    }
}
