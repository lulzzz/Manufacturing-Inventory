using ManufacturingInventory.InstallSequence.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ManufacturingInventory.InstallSequence {
    public class InstallSequenceModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider) {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion","WelcomeView");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<FileLocationView>();
            containerRegistry.RegisterForNavigation<FinishedView>();
            containerRegistry.RegisterForNavigation<InstallingView>();
            containerRegistry.RegisterForNavigation<WelcomeView>();
            //containerRegistry.RegisterForNavigation(typeof(FinishedView), "FinishedView");
            //containerRegistry.RegisterForNavigation(typeof(InstallingView), "InstallingView");
        }
    }
}