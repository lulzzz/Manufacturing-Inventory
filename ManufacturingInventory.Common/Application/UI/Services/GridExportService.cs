using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.Services {
    public class GridExportService : ServiceBase, IExportService {

        public void Export(Stream stream, ExportFormat format, XlsxExportOptionsEx options = null) {
            GridControl grid = (GridControl)AssociatedObject;

            switch (format) {
                case ExportFormat.Xlsx: {
                    if (options != null) {
                        grid.View.ExportToXlsx(stream, options);
                    } else {
                        grid.View.ExportToXlsx(stream);
                    }

                    break;
                }
                case ExportFormat.Pdf: {
                    grid.View.ExportToPdf(stream);
                    break;
                }
                case ExportFormat.Csv: {
                    grid.View.ExportToCsv(stream);
                    break;
                }
            }
        }

    }
}
