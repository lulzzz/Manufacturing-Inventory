using System;
using System.ComponentModel;
using System.Globalization;


namespace ManufacturingInventory.Extensions {
    public static class EnumHelper {

        /// <summary>
        /// Returns description attribute of enum.  If description not foud returns null
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <param name="e">Enum to get description from</param>
        /// <returns></returns>
        public static string GetDescription<T>(this T e) where T : IConvertible {
            string description = null;
            if (e is Enum) {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);
                foreach (int val in values) {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture)) {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAtt = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAtt.Length > 0) {
                            description = ((DescriptionAttribute)descriptionAtt[0]).Description;
                        }
                        break;
                    }
                }
            }
            return description;
        }


    }
}
