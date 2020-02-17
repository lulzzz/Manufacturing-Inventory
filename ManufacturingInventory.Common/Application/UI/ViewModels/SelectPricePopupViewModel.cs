using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.ViewModels {
    public class SelectPricePopupViewModel {

        public SelectPricePopupViewModel() {
            this.Prices = new ObservableCollection<Price>();
        }

        public SelectPricePopupViewModel(IEnumerable<Price> prices) {
            this.Prices = new ObservableCollection<Price>(prices);  
        }

        public virtual ObservableCollection<Price> Prices { get; set; }
        public virtual Price SelectedPrice { get; set; }
        
    }
}
