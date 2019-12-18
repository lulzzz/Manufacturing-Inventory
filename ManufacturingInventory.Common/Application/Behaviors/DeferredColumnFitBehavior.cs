using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManufacturingInventory.Common.Application.Behaviors {
    public class DeferredColumnFitBehavior : Behavior<GridControl> {
        private Task _task;
        GridControl AssociatedGrid { get { return AssociatedObject; } }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedGrid.ItemsSourceChanged += OnItemsSourceChanged;
            AssociatedGrid.LayoutUpdated += OnLayoutUpdated;
        }

        protected override void OnDetaching() {
            AssociatedGrid.ItemsSourceChanged -= OnItemsSourceChanged;
            AssociatedGrid.LayoutUpdated -= OnLayoutUpdated;
            base.OnDetaching();
        }

        private void OnItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
            BestFit();
        }

        private void OnLayoutUpdated(object sender, EventArgs e) {
            BestFit();
        }

        private void BestFit() {
            if (_task != null && !_task.IsCompleted) return;

            _task = Task.Factory
                .StartNew(() => Thread.Sleep(10))
                .ContinueWith(t => ((TableView)AssociatedGrid.View).BestFitColumns(),
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
