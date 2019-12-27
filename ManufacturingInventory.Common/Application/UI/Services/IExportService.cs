using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManufacturingInventory.Common.Application.UI.Services {
    public enum ExportFormat { Xlsx, Pdf, Csv }

    public interface IExportService {
        void Export(Stream stream, ExportFormat format, XlsxExportOptionsEx options = null);
    }
}
