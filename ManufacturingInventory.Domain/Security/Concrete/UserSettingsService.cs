using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using Serilog;

namespace ManufacturingInventory.Domain.Security.Concrete {
    public class UserSettingsService : IUserSettingsService {
        private ILogger _logger;

        public UserSettingsService(ILogger logger,ManufacturingContext context) {
            this._logger = logger;
        }

        public async Task<UserSettings> ReadUserSettings() {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings", "UserSettings.xml");
            this._logger.Information("Settings Path: {0}",path);
            if (File.Exists(path)) {
                var settings = await this.XmlDeSerializeUserSettings(path);
                var encrypted = new EncryptedResult();
                encrypted.Key = Convert.FromBase64String(settings.Key);
                encrypted.IV = Convert.FromBase64String(settings.IV);
                encrypted.Encrypted = Convert.FromBase64String(settings.Password);
                settings.Password = await EncryptionService.Decrypt(encrypted);
                return settings;
            } else {
                this._logger.Error("Failed to read UserSettings,IUserSettingsService");
                return null;
            }
        }
        
        
        public async Task<bool> SaveUserSettings(UserSettings userSettings) {
            bool error = false;
            var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppSettings");

            this._logger.Information("Settings Directory: {0}", settingsPath);
            
            if (!Directory.Exists(settingsPath)) {
                try {
                    Directory.CreateDirectory(settingsPath);
                } catch {
                    this._logger.Error("Failed to create directory");
                    return false;
                }
            }
            var path = Path.Combine(settingsPath, "UserSettings.xml");
            await Task.Run(() => {
                XmlSerializer writer = new XmlSerializer(typeof(UserSettings));
                try {
                    FileStream fileStream = File.Create(path);
                    writer.Serialize(fileStream, userSettings);
                }catch(Exception e) {
                    this._logger.Error(e, "Failed to write out user settings, IUserSettingsService");
                    error = true;
                }
            });
            return !error;
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
