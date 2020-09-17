using ManufacturingInventory.LocationManagment.Internal;
using ManufacturingInventory.LocationManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.LocationManagment {
    public class LocationManagmentModule : IModule {

        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(LocalRegions.LocationNavigationRegion, typeof(LocationManagmentNavigationView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<LocationManagmentMainView>(ModuleViews.LocationManagmentMainView);
            containerRegistry.RegisterForNavigation<LocationManagmentDetailsView>(ModuleViews.LocationManagmentDetailsView);
        }
    }
}