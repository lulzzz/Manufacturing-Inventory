using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using DevExpress.Mvvm;
using PrismCommands = Prism.Commands;
using Prism.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ManufacturingInventory.PartsManagment.Internal;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;

namespace ManufacturingInventory.PartsManagment.ViewModels {
    //public class PriceDetailsViewModel:InventoryViewModelBase {

    //    protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("PartInstanceDetailsMessageService"); }
    //    protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("PartInstanceDetailDispatcher"); }



    //    private IEventAggregator _eventAggregator;
    //    private IRegionManager _regionManager;
    //    //private IPartInstanceDetailsEditUseCase _editInstance;

    //    private ObservableCollection<Attachment> _attachments;
    //    private ObservableCollection<Distributor> _distributors;

    //    public override bool KeepAlive => false;
    //}
}
