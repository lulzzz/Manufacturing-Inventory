using ManufacturingInventory.Common.Application;
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
            //regionManager.RegisterViewWithRegion(Regions.PartDetailsRegion, typeof(PartsDetailView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry){
            containerRegistry.RegisterForNavigation<PartsManagmentMainView>();
            containerRegistry.RegisterForNavigation<PartsDetailView>();
            containerRegistry.RegisterForNavigation<PartInstanceDetailsView>();
        }
    }
}