using System;
using System.ComponentModel;
using System.Globalization;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Domain.Extensions {
    public static class EnumHelper {
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

        public static CategoryTypes GetCategoryType(this CategoryOption categoryOption) {
            switch (categoryOption) {
                case CategoryOption.Condition:
                    return CategoryTypes.Condition;
                case CategoryOption.StockType:
                    return CategoryTypes.StockType;
                case CategoryOption.Organization:
                    return CategoryTypes.Organization;
                case CategoryOption.Usage:
                    return CategoryTypes.Usage;
                case CategoryOption.NotSelected:
                    return CategoryTypes.InvalidType;
                default:
                    return CategoryTypes.InvalidType;
            }
        }

        public static CategoryOption GetCategoryOption(this CategoryTypes categoryType) {
            switch (categoryType) {
                case CategoryTypes.Organization:
                    return CategoryOption.Organization;
                case CategoryTypes.Condition:
                    return CategoryOption.Condition;
                case CategoryTypes.StockType:
                    return CategoryOption.StockType;
                case CategoryTypes.Usage:
                    return CategoryOption.Usage;
                case CategoryTypes.InvalidType:
                    return CategoryOption.NotSelected;
                default:
                    return CategoryOption.NotSelected;
            }
        }
    }
}
