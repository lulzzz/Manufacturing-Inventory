using DevExpress.Mvvm;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using PublishTool.Common;
using System;
using System.Threading.Tasks;

namespace PublishTool.ViewModels {

    public class MainWindowViewModel : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices, IRegionMemberLifetime {

        #region ServiceContainerRegion
        public IServiceContainer _serviceContainer = null;
        IServiceContainer ISupportServices.ServiceContainer { get { return ServiceContainer; } }
        protected IServiceContainer ServiceContainer {
            get {
                if (this._serviceContainer == null) {
                    this._serviceContainer = new ServiceContainer(this);
                }
                return this._serviceContainer;
            }
        }
        #endregion

        protected IMessageBoxService MessageBoxService { get => ServiceContainer.GetService<IMessageBoxService>("MessageBoxService"); }
        protected IDispatcherService DispatcherService { get => ServiceContainer.GetService<IDispatcherService>("DispatcherService"); }
        public IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>("OpenFileDialog"); } }
        public IFolderBrowserDialogService FolderDialogService { get { return ServiceContainer.GetService<IFolderBrowserDialogService>("SelectFolderDialog"); } }

        IEventAggregator _eventAggregator;
        IPublishToolService _publishService;

        private string _log;
        private string _outputPath;
        private string _projectPath;

        public AsyncCommand PublishCommand { get; private set; }
        public AsyncCommand CancelCommand { get; private set; }
        public AsyncCommand SelectProjectCommand { get; private set; }
        public AsyncCommand ChangeOutputLocationCommand { get; private set; }

        public MainWindowViewModel(IEventAggregator eventAggregator,IPublishToolService publishService) {
            this._eventAggregator = eventAggregator;
            this._publishService = publishService;
            this.PublishCommand = new AsyncCommand(this.PublishHandler);
            this._eventAggregator.GetEvent<UpdateLogEvent>().Subscribe(async (message) => await this.UpdateLogHandler(message));
        }

        public bool KeepAlive { get => false; }

        public string Log {
            get => this._log;
            set => SetProperty(ref this._log, value);
        }

        public string OutputPath { 
            get => this._outputPath;
            set => SetProperty(ref this._outputPath, value);
        }

        public string ProjectPath { 
            get => this._projectPath; 
            set => SetProperty(ref this._projectPath,value);
        }

        public async Task PublishHandler() {
            await this._publishService.Publish();
        }

        public async Task UpdateLogHandler(string message) {
            await Task.Run(() => {
                this.DispatcherService.BeginInvoke(() => {
                    this.Log += message + Environment.NewLine;
                });
            });
        }
    }
}
