using Prism.Ioc;
using ManufacturingInventory.Main.Views;
using System.Windows;
using ManufacturingInventory.Main.ViewModels;

namespace ManufacturingInventory.Main {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {

        protected override Window CreateShell() {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {

        }

        protected override void OnStartup(StartupEventArgs e) {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (this.CheckVersion()) {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            } else {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.Shutdown();
            }
            base.OnStartup(e);
        }

        private bool CheckVersion() {
            CheckVersionWindow versionWindow = new CheckVersionWindow();
            var versionVM = new CheckVersionViewModel();
            versionVM.CheckCompleted += (sender, args) => {
                versionWindow.Close();
            };
            versionWindow.DataContext = versionVM;
            versionWindow.ShowDialog();
            return true;
        }
    }
}
