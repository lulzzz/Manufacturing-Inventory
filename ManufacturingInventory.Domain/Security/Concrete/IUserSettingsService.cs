using ManufacturingInventory.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Domain.Security.Concrete {
    public interface IUserSettingsService {
        Task SaveUserSettings(UserSettings settings);
        Task<UserSettings> ReadUserSettings();
    }
}
