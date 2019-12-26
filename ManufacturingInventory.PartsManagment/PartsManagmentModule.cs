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
            regionManager.RegisterViewWithRegion(Regions.PartsNavigationRegion, typeof(PartsNavigationView));
            regionManager.RegisterViewWithRegion("PartTransactionTableRegion", typeof(TransactionTableView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry){
            containerRegistry.RegisterForNavigation<PartsManagmentMainView>(AppViews.PartsManagmentMainView);
            containerRegistry.RegisterForNavigation<PartsDetailView>(AppViews.PartsDetailView);
            containerRegistry.RegisterForNavigation<PartInstanceDetailsView>(AppViews.PartInstanceDetailsView);
            containerRegistry.RegisterForNavigation<TransactionDetailsView>(AppViews.TransactionDetailsView);
        }
    }
}