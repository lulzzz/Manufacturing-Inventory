using ManufacturingInventory.Common.Application.UI.Views;
using ManufacturingInventory.DistributorManagment.Internal;
using ManufacturingInventory.DistributorManagment.Views;
using ManufacturingInventory.Common.Application;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.DistributorManagment {
    public class DistributorManagmentModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(LocalRegions.DistributorNavigationRegion, typeof(DistributorNavigationView));
            regionManager.RegisterViewWithRegion(LocalRegions.AttachmentTableRegion, typeof(AttachmentsTableView));
            regionManager.RegisterViewWithRegion(LocalRegions.ContactRegion, typeof(ContactTableDetailView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<DistributorDetailsView>(ModuleViews.DistributorDetailsView);
            containerRegistry.RegisterForNavigation<DistributorMainView>(MainAppViews.DistributorMainView);
        }
    }
}