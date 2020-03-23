using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using ManufacturingInventory.Application.Boundaries;

namespace ManufacturingInventory.CategoryManagment.ViewModels {
    public class CategoryNavigationViewModel : InventoryViewModelBase {
        public override bool KeepAlive => throw new System.NotImplementedException();
    }
}
