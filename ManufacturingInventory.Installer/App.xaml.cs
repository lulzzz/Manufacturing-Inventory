using Prism.Ioc;
using ManufacturingInventory.Installer.Views;
using System.Windows;
using Prism.Regions;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Prism;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Prism.Modularity;
using ManufacturingInventory.InstallSequence;
using ManufacturingInventory.InstallSequence.Infrastructure;

namespace ManufacturingInventory.Installer {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        private VersionCheckerResponce installTraveler;

        protected override Window CreateShell() {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e) {
            ThemeManager.ApplicationThemeChanged += this.ThemeManager_ApplicationThemeChanged;
            GridControl.AllowInfiniteGridSize = true;
            this.installTraveler = VersionChecker.CheckInstalledVersion();

            //Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            //if () {
            //    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //} else {
            //    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //    this.Shutdown();
            //}
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            if (this.installTraveler != null) {
                containerRegistry.RegisterInstance<VersionCheckerResponce>(this.installTraveler);
            } else {
                containerRegistry.RegisterInstance<VersionCheckerResponce>(new VersionCheckerResponce(InstallStatus.NotInstalled,"",""));
            }
            containerRegistry.Register<IInstaller,InstallSequence.Infrastructure.Installer>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<InstallSequenceModule>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings) {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            var factory = base.Container.Resolve<IRegionBehaviorFactory>();
            regionAdapterMappings.RegisterMapping(typeof(DocumentGroup), AdapterFactory.Make<RegionAdapterBase<DocumentGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutPanel), AdapterFactory.Make<RegionAdapterBase<LayoutPanel>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutGroup), AdapterFactory.Make<RegionAdapterBase<LayoutGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(TabbedGroup), AdapterFactory.Make<RegionAdapterBase<TabbedGroup>>(factory));
        }

        private void ThemeManager_ApplicationThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
            ApplicationThemeHelper.SaveApplicationThemeName();
        }
    }
}
