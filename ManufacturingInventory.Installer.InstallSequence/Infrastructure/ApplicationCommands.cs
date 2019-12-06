using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.InstallSequence.Infrastructure {
    public interface IApplicationCommands {
        CompositeCommand CancelCommand { get; }
    }

    public class ApplicationCommands:IApplicationCommands {
        private CompositeCommand _cancelCommand = new CompositeCommand(true);

        public CompositeCommand CancelCommand {
            get => this._cancelCommand;
        }

    }
}
