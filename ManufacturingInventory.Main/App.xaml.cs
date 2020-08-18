using ManufacturingInventory.Application.Boundaries.AttachmentsEdit;
using ManufacturingInventory.Application.Boundaries.Authentication;
using ManufacturingInventory.Application.Boundaries.CategoryBoundaries;
using ManufacturingInventory.Application.Boundaries.CheckIn;
using ManufacturingInventory.Application.Boundaries.Checkout;
using ManufacturingInventory.Application.Boundaries.ContactTableDetailEdit;
using ManufacturingInventory.Application.Boundaries.DistributorManagment;
using ManufacturingInventory.Application.Boundaries.PartDetails;
using ManufacturingInventory.Application.Boundaries.PartInstanceDetailsEdit;
using ManufacturingInventory.Application.Boundaries.PartInstanceTableView;
using ManufacturingInventory.Application.Boundaries.PartNavigationEdit;
using ManufacturingInventory.Application.Boundaries.PriceEdit;
using ManufacturingInventory.Application.Boundaries.ReturnItem;
using ManufacturingInventory.Application.Boundaries.TransactionTableEdit;
using ManufacturingInventory.Application.Boundaries.ReportingBoundaries;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.CategoryManagment;
using ManufacturingInventory.DistributorManagment;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Domain.Security.Concrete;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Reporting;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.ManufacturingApplication.Services;
using ManufacturingInventory.ManufacturingApplication.ViewModels;
using ManufacturingInventory.ManufacturingApplication.Views;
using ManufacturingInventory.PartsManagment;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Prism;
using DryIoc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using FastExpressionCompiler.LightExpression;
using Logger =Serilog.ILogger;

namespace ManufacturingInventory.ManufacturingApplication {
    public partial class App {
        private IUserService userService = new UserService();
        private Logger _logger;
        public IConfiguration Configuration { get; private set; }
        public String ConnectionString { get; private set; }
        public DbContextOptionsBuilder<ManufacturingContext> optionsBuilder { get; private set; }

        protected override Window CreateShell() {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e) {
            DXSplashScreen.Show<ManufacturingInventory.ManufacturingApplication.SETSplashScreen>();
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            this.Configuration = builder.Build();
            this.optionsBuilder = new DbContextOptionsBuilder<ManufacturingContext>();
            this.optionsBuilder.UseSqlServer(this.Configuration.GetConnectionString("InventoryConnection_home"));

            ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017BlueName;
            //ApplicationThemeHelper.UpdateApplicationThemeName();
            //ThemeManager.ApplicationThemeChanged += this.ThemeManager_ApplicationThemeChanged;
            //DXTabControl.TabContentCacheModeProperty = TabContentCacheMode.CacheTabsOnSelecting;
            GridControl.AllowInfiniteGridSize = true;
            //this.CheckVersion_v2();
            this.CreateLogger();
            this.ManualLogIn();

            //this.Login();
            base.OnStartup(e);
        }

        private void Login() {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (this.ShowLogin()) {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            } else {
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.Shutdown();
            }
        }

        private void CheckVersion() {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (this.ShowCheckVersionWindow()) {
                Process process = new Process();
                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = @"C:\Program Files (x86)\Manufacturing Inventory\Installer\InventoryInstaller.exe",
                    UseShellExecute = false,
                };
                process.StartInfo = psi;
                process.Start();
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.Shutdown();
            } else {
                if (!DXSplashScreen.IsActive)
                    DXSplashScreen.Show<ManufacturingInventory.ManufacturingApplication.SETSplashScreen>();

                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
        }

        private void ManualLogIn() {
            using var context = new ManufacturingContext(this.optionsBuilder.Options);

            var user = context.Users
                .Include(e => e.Sessions)
                    .ThenInclude(e => e.Transactions)
                .Include(e => e.Permission)
                .FirstOrDefault(e => e.FirstName == "Andrew");

            if (user != null) {
                Session session = new Session(user);
                context.Sessions.Add(session);
                context.SaveChanges();
                this.userService.CurrentUserName = user.UserName;
                this.userService.CurrentSessionId = session.Id;
                this.userService.UserPermissionName = user.Permission.Name;
            }
        }

        private bool ShowLogin() {
            //Startup Login
            LoginWindow loginWindow = new LoginWindow();
            DomainManager domainManager = new DomainManager();
            ManufacturingContext context = new ManufacturingContext(this.optionsBuilder.Options);
            IUserSettingsService userSettingsService = new UserSettingsService(this._logger, context);
            IAuthenticationUseCase auth = new AuthenticationService(context, domainManager, this._logger,userSettingsService);
            var loginVM = new LoginViewModel(auth,userSettingsService);
            loginVM.LoginCompleted += (sender, args) => {
                if (loginVM.LoginResponce.Success) {
                    this.userService = loginVM.LoginResponce.Service;
                    DXSplashScreen.Show<SETSplashScreen>();
                }
                loginWindow.Close();

            };
            loginWindow.DataContext = loginVM;
            if (DXSplashScreen.IsActive)
                DXSplashScreen.Close();

            loginWindow.ShowDialog();
            return this.userService.IsValid();
        }

        private bool ShowCheckVersionWindow() {
            CheckVersion versionChecker = new CheckVersion();
            var response = versionChecker.Check();
            if (response.NewVersionAvailable) {
                CheckVersionWindow versionWindow = new CheckVersionWindow();
                var versionVM = new CheckVersionViewModel(response);
                versionVM.CheckCompleted += (sender, args) => {
                    versionWindow.Close();
                };
                versionWindow.DataContext = versionVM;
                if (DXSplashScreen.IsActive)
                    DXSplashScreen.Close();

                versionWindow.ShowDialog();
                return versionVM.Update;
            } else {
                return false;
            }
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) {
            moduleCatalog.AddModule<PartsManagmentModule>();
            moduleCatalog.AddModule<DistributorManagmentModule>();
            moduleCatalog.AddModule<CategoryManagmentModule>();
            moduleCatalog.AddModule<ReportingModule>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            if (this.userService.IsValid()) {
                var container = containerRegistry.GetContainer();
                container.With(rules => rules.WithoutImplicitCheckForReuseMatchingScope());

                container.Register<ManufacturingContext>(setup: Setup.With(allowDisposableTransient: true),
                    made: Made.Of(()=>new ManufacturingContext(Arg.Index<DbContextOptions<ManufacturingContext>>(0)),requestIgnored=>this.optionsBuilder.Options));

                container.Register<IUnitOfWork, UnitOfWork>(setup: Setup.With(allowDisposableTransient: true));
                container.Register<ICheckOutUseCase, CheckOut>();
                container.Register<ICheckInUseCase, CheckIn>();
                container.Register<IPartNavigationEditUseCase, PartNavigationEdit>();
                container.Register<IPartSummaryEditUseCase, PartSummaryEdit>();
                container.Register<IPartInstanceDetailsEditUseCase, PartInstanceDetailsEdit>();
                container.Register<IAttachmentEditUseCase, AttachmentEdit>();
                container.Register<ITransactionTableUndoUseCase, TransactionTableEdit>();
                container.Register<IReturnItemUseCase, ReturnItem>();
                container.Register<IPriceEditUseCase, PriceEdit>();
                container.Register<IPartInstanceTableViewUseCase, PartInstanceTableViewUseCase>();
                container.Register<IDistributorEditUseCase, DistributorEdit>();
                container.Register<IContactTableDetailEditUseCase, ContactTableDetailEdit>();
                container.Register<ICategoryEditUseCase, CategoryEdit>();
                container.Register<IAuthenticationUseCase, AuthenticationService>();
                container.Register<IMonthlyReportUseCase, MonthlySummaryUseCase>();
                container.Register<ICurrentInventoryUseCase, CurrentInventoryUseCase>();
                container.Register<ITransactionLogUseCase, TransactionLogUseCase>();
                container.Register<ILogInService, LogInService>();
                container.Register<IDomainManager, DomainManager>();
                container.RegisterInstance<IUserService>(this.userService);
                //this.CreateLogger();
                container.RegisterInstance<Logger>(this._logger);
            } else {
                this.Shutdown();
            }
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings) {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            var factory = base.Container.Resolve<IRegionBehaviorFactory>();
            regionAdapterMappings.RegisterMapping(typeof(DocumentGroup), AdapterFactory.Make<RegionAdapterBase<DocumentGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutPanel), AdapterFactory.Make<RegionAdapterBase<LayoutPanel>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(LayoutGroup), AdapterFactory.Make<RegionAdapterBase<LayoutGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(TabbedGroup), AdapterFactory.Make<RegionAdapterBase<TabbedGroup>>(factory));
            regionAdapterMappings.RegisterMapping(typeof(DXTabControl), AdapterFactory.Make<RegionAdapterBase<DXTabControl>>(factory));
        }

        private void CreateLogger() {
            const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logfile = Path.Combine(baseDir, "App_Data", "logs", "log.txt");
            this._logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.Console(LogEventLevel.Information, loggerTemplate, theme: AnsiConsoleTheme.Literate), blockWhenFull: true)
                .WriteTo.Async(a => a.File(logfile, LogEventLevel.Information, loggerTemplate,rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90),blockWhenFull:true)
                .CreateLogger();

            this._logger.Information("====================================================================");
            this._logger.Information($"Application Starts. Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");
            this._logger.Information($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
            this._logger.Information("====================================================================\r\n");
        }

        private void ThemeManager_ApplicationThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
            ApplicationThemeHelper.SaveApplicationThemeName();
        }
    }
}
