using ManufacturingInventory.Reporting.Views;
using ManufacturingInventory.Reporting.Internal;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.Reporting {
    public class ReportingModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion(LocalRegions.ReportingMainRegion, typeof(ReportingMainView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ReportingMainView>(ModuleViews.ReportingMainView);
            containerRegistry.RegisterForNavigation<ReportingMonthlySummaryView>(ModuleViews.ReportingMonthlySummaryView);
        }
    }
}