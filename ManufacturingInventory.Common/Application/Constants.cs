using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Application {
    public static class Constants {
        public static readonly string DestinationDirectory = @"\\172.20.4.20\inventory_attachments";
        public static readonly string FileFilters =
                "Word Documents|*.doc|Excel Worksheets|*.xls|PowerPoint Presentations|*.ppt|" +
                "Office Files|*.doc;*.xls;*.ppt|" +
                "PDF Files|*.pdf|" +
                "All Images|*.BMP;*.DIB;*.RLE;*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG|" +
                "BMP Files: (*.BMP;*.DIB;*.RLE)|*.BMP;*.DIB;*.RLE|" +
                "JPEG Files: (*.JPG;*.JPEG;*.JPE;*.JFIF)|*.JPG;*.JPEG;*.JPE;*.JFIF|" +
                "GIF Files: (*.GIF)|*.GIF|" +
                "TIFF Files: (*.TIF;*.TIFF)|*.TIF;*.TIFF|" +
                "PNG Files: (*.PNG)|*.PNG|" +
                "All Files|*.*";
        public static readonly string DefaultNewProductName = "Enter Product Number Here";
        public static readonly string DefaultLotNumber = "Enter Lot Number Here";
        public static readonly string DefaultSupplierPo = "Enter Supplier Po Number Here";
        public static readonly string DefaultRankName = "Enter Rank/Bin Here";
        public static readonly int DefaultStockId = 14;
    }

    public static class AlertTypes {
        public static string IndividualAlert { get => "individual_alert"; }
        public static string CombinedAlert { get => "combined_alert"; }
    }
}
