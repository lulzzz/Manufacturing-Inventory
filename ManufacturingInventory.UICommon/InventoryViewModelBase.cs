using Prism.Regions;
using DevExpress.Mvvm;
using System;

namespace ManufacturingInventory.UICommon {
    public abstract class InventoryViewModelBase : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices, IRegionMemberLifetime {
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

        public abstract bool KeepAlive { get; }
    }

    public abstract class InventoryViewModelNavigationBase : Prism.Mvvm.BindableBase, DevExpress.Mvvm.ISupportServices,INavigateAsync ,INavigationAware, IRegionMemberLifetime {
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

        public abstract bool KeepAlive { get; }
        public abstract void OnNavigatedTo(NavigationContext navigationContext);
        public abstract bool IsNavigationTarget(NavigationContext navigationContext);
        public abstract void OnNavigatedFrom(NavigationContext navigationContext);
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback) => throw new NotImplementedException();
        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters) => throw new NotImplementedException();
    }
}
