using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ManufacturingInventory.Extensions {
    /// <summary>
    /// Gets enum from string.  string much be a description of a known enum
    /// if can't find description returns default parameter
    /// </summary>
    public static class StringExtensions {

        public static T GetEnum<T>(this string value, T defaultIfFail) where T : IConvertible {
            if (string.IsNullOrEmpty(value)) {
                return defaultIfFail;
            }

            Type type = typeof(T);
            Array values = Enum.GetValues(type);
            foreach (T val in values) {

                var memInfo = type.GetMember(type.GetEnumName(val));
                var descriptionAtt = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAtt.Length > 0) {
                    if (value == ((DescriptionAttribute)descriptionAtt[0]).Description) {
                        return val;
                    }
                }
            }
            return defaultIfFail;
        }
    }
}
