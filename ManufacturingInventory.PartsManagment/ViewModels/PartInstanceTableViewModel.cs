using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.PartsManagment.Internal;
using Prism.Events;
using Prism.Regions;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    public class PartInstanceTableViewModel {

        private ObservableCollection<PartInstance> _partInstances;
        private bool _isEdit;

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;


    }
}
