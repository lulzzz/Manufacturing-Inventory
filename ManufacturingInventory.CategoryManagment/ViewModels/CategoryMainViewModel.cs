using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Common.Application.UI.ViewModels;
using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Common.Application.ValueConverters;
using System.IO;
using System.Collections.Generic;

namespace ManufacturingInventory.CategoryManagment.ViewModels {
    public class CategoryMainViewModel : InventoryViewModelBase {
        public override bool KeepAlive => true;
    }
}
