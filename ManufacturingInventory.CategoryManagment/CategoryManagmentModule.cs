using ManufacturingInventory.CategoryManagment.Internal;
using ManufacturingInventory.CategoryManagment.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.CategoryManagment {
    public class CategoryManagmentModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(LocalRegions.CategoryNavigationRegion, typeof(CategoryNavigationView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<CategoryDetailsView>(ModuleViews.CategoryDetailsView);
            containerRegistry.RegisterForNavigation<CategoryMainView>(ModuleViews.CategoryMainView);
        }
    }
}