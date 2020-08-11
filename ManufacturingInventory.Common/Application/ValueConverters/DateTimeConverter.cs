using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ManufacturingInventory.Common.Application.ValueConverters {
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateTimeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string strValue = value as string;
            return System.Convert.ToDateTime(value + " 01,1900");
            //if (DateTime.TryParse(strValue, out resultDateTime)) {
            //    return resultDateTime;
            //}
            //return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            DateTime month = (DateTime)value;
            string monthstr;
            try {
                monthstr = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Month);
                return monthstr;
            } catch {
                monthstr = "January";
                return monthstr;
            }

        }
    }


}
