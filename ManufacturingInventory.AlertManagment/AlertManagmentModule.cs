using ManufacturingInventory.AlertManagment.Internal;
using ManufacturingInventory.AlertManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.AlertManagment {
    public class AlertManagmentModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(LocalRegions.AlertsAvailableRegion, typeof(AlertsAvailableView));
            regionManager.RegisterViewWithRegion(LocalRegions.AlertsExistingRegion, typeof(AlertsExistingView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<AlertsMainView>(ModuleViews.AlertsMainView);
        }
    }
}