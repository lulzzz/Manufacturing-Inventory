using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManufacturingInventory.Reporting.Views {
    /// <summary>
    /// Interaction logic for ReportingMonthlySummaryView.xaml
    /// </summary>
    public partial class ReportingMonthlySummaryView : UserControl {
        private bool _isactive;
        public string PanelCaption { get { return "Monthly Report"; } }

        public bool IsActive {
            get => this._isactive;
            set => this._isactive = value;
        }
        public ReportingMonthlySummaryView() {
            InitializeComponent();
            this.IsActive = true;
        }
    }
}
