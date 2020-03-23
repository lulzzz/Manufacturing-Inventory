using DevExpress.Mvvm;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Common.Application.UI.Services;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ManufacturingInventory.Domain.DTOs;
using System;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using System.Windows;
using System.Collections.Generic;

namespace ManufacturingInventory.CategoryManagment.ViewModels {
    public class CategoryDetailsViewModel : InventoryViewModelNavigationBase {
        public override bool KeepAlive => throw new NotImplementedException();

        public override bool IsNavigationTarget(NavigationContext navigationContext) => throw new NotImplementedException();
        public override void OnNavigatedFrom(NavigationContext navigationContext) => throw new NotImplementedException();
        public override void OnNavigatedTo(NavigationContext navigationContext) => throw new NotImplementedException();
    }
}
