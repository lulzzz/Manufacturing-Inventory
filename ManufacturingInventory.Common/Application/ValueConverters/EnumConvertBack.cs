using System;
using System.Windows.Markup;

namespace ManufacturingInventory.Common.Application.ValueConverters {
    public static class EnumConvertBack {
        public static object ConvertBack(object value,Type targetType,object parameter,System.Globalization.CultureInfo culture) {
            var enumType = value.GetType();
            var underlyingType = Enum.GetUnderlyingType(enumType);
            var numericValue = System.Convert.ChangeType(value, underlyingType);
            return numericValue;
        }

        public static T ConvertEnum<T>(object obj) where T : Enum {
            T enumVal = (T)Enum.ToObject(typeof(T), obj);
            return enumVal;
        }
    }

}
