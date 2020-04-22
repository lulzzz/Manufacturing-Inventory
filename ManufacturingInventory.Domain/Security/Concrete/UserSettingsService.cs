using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Castle.Core.Internal;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ManufacturingInventory.Domain.Security.Concrete {
    public class UserSettingsService : IUserSettingsService {
        private ILogger _logger;
        private ManufacturingContext _context;

        public UserSettingsService(ILogger logger,ManufacturingContext context) {
            this._logger = logger;
            this._context = context;
        }

        public async Task<UserSettings> ReadUserSettings() {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings", "UserSettings.xml");
            if (File.Exists(path)) {
                var settings = await this.XmlDeSerializeUserSettings(path);
                var encrypted = new EncryptedResult();
                encrypted.Key = Convert.FromBase64String(settings.Key);
                encrypted.IV = Convert.FromBase64String(settings.IV);
                encrypted.Encrypted = Convert.FromBase64String(settings.Password);
                settings.Password = await EncryptionService.Decrypt(encrypted);
                this._logger.Information("User Settings Read Successfully");
                return settings;
            } else {
                return null;
            }
        }
        
        
        public async Task<bool> SaveUserSettings(string userName,string password) {
            if (userName.IsNullOrEmpty()) {
                this._logger.Error("Username Null Or Empty, cannot save");
                return false;
            }else if (password.IsNullOrEmpty()) {
                this._logger.Error("Password Null Or Empty, cannot save");
                return false;
            }



            UserSettings userSettings = new UserSettings();
            userSettings.UserName = userName;
            var result = await EncryptionService.Encrypt(password);
            userSettings.Key = Convert.ToBase64String(result.Key);
            userSettings.IV = Convert.ToBase64String(result.IV);
            userSettings.Password = Convert.ToBase64String(result.Encrypted);
            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings");

            var user = await this._context.Users.FirstOrDefaultAsync(e => e.UserName == userName);
            if (user != null) {


            } else {

            }

            this._logger.Information("Settings Directory: {0}", settingsPath);
            
            if (!Directory.Exists(settingsPath)) {
                try {
                    Directory.CreateDirectory(settingsPath);
                    var path = Path.Combine(settingsPath, "UserSettings.xml");
                } catch {
                    this._logger.Error("Failed to create directory");
                    return false;
                }
            }



        }

        private async Task XmlSerializeUserSettings(UserSettings userSettings,string path) {
            await Task.Run(() => {
                XmlSerializer writer = new XmlSerializer(typeof(UserSettings));
                try {
                    FileStream fileStream = File.Create(path);
                    writer.Serialize(fileStream, userSettings);
                    this._logger.Information("UserSettings should be saved");
                } catch {
                    this._logger.Error("Failed to save user settings");
                }
            });

        }

        public async Task<UserSettings> XmlDeSerializeUserSettings(string path) {
            UserSettings settings = new UserSettings();
            bool error = false;
            await Task.Run(() => {
                XmlSerializer reader = new XmlSerializer(typeof(UserSettings));
                using (StreamReader streamReader = new StreamReader(path)) {
                    try {
                        settings = (UserSettings)reader.Deserialize(streamReader);
                    } catch {
                        this._logger.Error("Failed to read user settings");
                        error = true;
                    }
                }
            });
            return (error) ? null:settings;
        }
    }
}
