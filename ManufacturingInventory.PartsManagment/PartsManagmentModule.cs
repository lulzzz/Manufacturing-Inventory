using ManufacturingInventory.Common.Application;
using ManufacturingInventory.PartsManagment.Internal;
using ManufacturingInventory.PartsManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.PartsManagment
{
    public class PartsManagmentModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider){
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(LocalRegions.PartsNavigationRegion, typeof(PartsNavigationView));
            regionManager.RegisterViewWithRegion(LocalRegions.TransactionTableRegion, typeof(TransactionTableView));
            regionManager.RegisterViewWithRegion(LocalRegions.PartInstanceTableRegion, typeof(PartInstanceTableView));
            regionManager.RegisterViewWithRegion(LocalRegions.AttachmentTableRegion, typeof(AttachmentsTableView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry){
            containerRegistry.RegisterForNavigation<PartsManagmentMainView>(ModuleViews.PartsManagmentMainView);
            containerRegistry.RegisterForNavigation<PartsDetailView>(ModuleViews.PartsDetailView);
            containerRegistry.RegisterForNavigation<PartInstanceDetailsView>(ModuleViews.PartInstanceDetailsView);
            containerRegistry.RegisterForNavigation<TransactionDetailsView>(ModuleViews.TransactionDetailsView);
        }
    }
}