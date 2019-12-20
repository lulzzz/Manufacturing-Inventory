using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;

namespace ManufacturingInventory.PartsManagment.Internal {
    public interface IModuleCommands {
        CompositeCommand ViewDetailsCommand { get; }
    }

    public class TransactionCommands:IModuleCommands {
        private CompositeCommand _viewDetailsCommand = new CompositeCommand(true);

        public CompositeCommand ViewDetailsCommand {
            get => this._viewDetailsCommand;
        }

    }
}
