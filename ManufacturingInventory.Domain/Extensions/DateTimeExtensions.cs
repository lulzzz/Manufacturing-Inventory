using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ManufacturingInventory.Domain.Extensions {
    public static class DateTimeExtensions {
        public static string MonthName(this DateTime dateTime) {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string ShortMonthName(this DateTime dateTime) {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }
    }
}
