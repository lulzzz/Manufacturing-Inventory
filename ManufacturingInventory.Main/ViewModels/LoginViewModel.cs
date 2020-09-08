using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm;
using Effortless.Net.Encryption;
using ManufacturingInventory.Common.Application;
using ManufacturingInventory.Domain.Buisness.Concrete;
using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Application.Boundaries.Authentication;
using ManufacturingInventory.Domain.Security.Concrete;
using Castle.Core.Internal;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace ManufacturingInventory.ManufacturingApplication.ViewModels {
    public class LoginViewModel : InventoryViewModelBase {
        protected IMessageBoxService MessageBoxService { get { return ServiceContainer.GetService<IMessageBoxService>("Error"); } }
        private IAuthenticationUseCase _authenticationService;
        private IUserSettingsService _userSettingsService;
        private List<string> _versions = new List<string>();
        private string _selectedVersion;
        private string userName;
        private string password;
        private bool saveLogin;

        public event EventHandler LoginCompleted;

        //public string Username { get; set; }
        //public string Password { get; set; }
        //public bool SaveLogin { get; set; }

        public List<string> Versions {
            get => this._versions;
            set => SetProperty(ref this._versions, value, "Versions");
        }

        public string SelectedVersion {
            get => this._selectedVersion;
            set => SetProperty(ref this._selectedVersion, value, "SelectedVersion");
        }

        public LogInResponce LoginResponce { get; set; }
        public AsyncCommand LoginCommand { get; private set; }
        public AsyncCommand InitializeCommand { get; private set; }
        public Prism.Commands.DelegateCommand CancelCommand { get; private set; }

        public LoginViewModel(IAuthenticationUseCase authenticationService,IUserSettingsService userSettingsService) {
            this._authenticationService = authenticationService;
            this._userSettingsService = userSettingsService;
            this.LoginCommand = new AsyncCommand(this.LogIn);
            this.CancelCommand = new Prism.Commands.DelegateCommand(this.Cancel);
            this.InitializeCommand = new AsyncCommand(this.Load);
        }


        private async Task LogIn() {
            if(!this.Username.IsNullOrEmpty() && !this.Password.IsNullOrEmpty()) {
                AuthenticationInput input = new AuthenticationInput(this.Username, this.Password, AuthenticationActions.LOGIN, this.SaveLogin);
                var result = await this._authenticationService.Execute(input);
                if (result.Success) {
                    this.LoginResponce = new LogInResponce(result.UserService, result.Success);
                    AsyncContext.Run(()=>this.LoginCompleted?.Invoke(this, EventArgs.Empty));
                } else {
                    this.LoginResponce = new LogInResponce(result.Message);
                    this.ShowErrorMessage(this.LoginResponce);
                }
            } else {
                this.ShowErrorMessage();
            }
        }

        private void ShowErrorMessage(LogInResponce responce = null) {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Error: ");
            if (responce != null) {
                message.AppendLine("Message: " + responce.Message);
            } else {
                if (string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password)) {
                    message.AppendLine("Username and Password must be filled in to continue");
                } else if (string.IsNullOrEmpty(this.Username)) {
                    message.AppendLine("Username must be filled in to continue");
                } else if (string.IsNullOrEmpty(this.Password)) {
                    message.AppendLine("Password must be filled in to continue");
                }
            }
            this.MessageBoxService.ShowMessage(message.ToString());
        }

        private void Cancel() {
            this.LoginResponce = new LogInResponce(null, false);
            //this.LoginCompleted?.Invoke(this, EventArgs.Empty);
            this.LoginCompleted?.Invoke(this, EventArgs.Empty);
            //AsyncContext.Run(() => this.LoginCompleted?.Invoke(this, EventArgs.Empty));
        }

        private async Task Load() {
            var userSettings = await this._userSettingsService.ReadUserSettings();
            if (userSettings != null) {
                this.SaveLogin = true;
                this.Username = userSettings.UserName;
                this.Password = userSettings.Password;
            }
        }

        public override bool KeepAlive {
            get => false;
        }
        public string Username { get => this.userName; set => SetProperty(ref this.userName,value); }
        public string Password { get => this.password; set => SetProperty(ref this.password, value); }
        public bool SaveLogin { get => this.saveLogin; set => SetProperty(ref this.saveLogin, value); }

        #region OldVersion

        //private async Task LogIn() {

        //    //if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password)) {
        //    //    this.LoginResponce = this._loginService.LogInWithPassword(this.Username, this.Password, this.SaveLogin);
        //    //    if (this.LoginResponce.Success) {
        //    //        if (this.SaveLogin) {
        //    //            if (!this.CheckProperties(this.LoginResponce.Service)) {
        //    //                this.SaveCredentials(this.LoginResponce.Service);
        //    //            }
        //    //        } else {
        //    //            Properties.Settings.Default.Reset();
        //    //        }
        //    //    } else {
        //    //        this.ShowErrorMessage(this.LoginResponce);
        //    //    }
        //    //    this.LoginCompleted?.Invoke(this, EventArgs.Empty);
        //    //} else {
        //    //    this.ShowErrorMessage();
        //    //}
        //}

        //private void LoadDefault() {

        //    //var saved = Properties.Settings.Default.CredentialsSaved;
        //    //if (saved) {
        //    //    this.Username = Properties.Settings.Default.UserName;
        //    //    var key = Properties.Settings.Default.userKey;
        //    //    var iv = Properties.Settings.Default.userIV;
        //    //    var encrypted = Properties.Settings.Default.Password;
        //    //    this.Password = Strings.Decrypt(encrypted, key, iv);
        //    //    this.SaveLogin = saved;
        //    //}
        //}

        //private bool CheckProperties(IUserService service) {
        //    //if (Properties.Settings.Default.CredentialsSaved) {
        //    //    var userName = Properties.Settings.Default.UserName;
        //    //    var key = Properties.Settings.Default.userKey;
        //    //    var iv = Properties.Settings.Default.userIV;
        //    //    var encrypted = Properties.Settings.Default.Password;
        //    //    bool credentialsSame = (service.CurrentUser.UserName.Equals(userName) &&
        //    //        service.CurrentUser.EncryptedPassword.Equals(encrypted) &&
        //    //        service.CurrentUser.Key.SequenceEqual(key) &&
        //    //        service.CurrentUser.IV.SequenceEqual(iv));
        //    //    return credentialsSame;
        //    //} else {
        //    //    return false;
        //    //}
        //    return true;
        //}

        //private void SaveCredentials(IUserService service) {
        //    //Properties.Settings.Default.UserName = service.CurrentUser.UserName;
        //    //Properties.Settings.Default.userKey = service.CurrentUser.Key;
        //    //Properties.Settings.Default.userIV = service.CurrentUser.IV;
        //    //Properties.Settings.Default.Password = service.CurrentUser.EncryptedPassword;
        //    //Properties.Settings.Default.CredentialsSaved = true;
        //    //Properties.Settings.Default.Save();
        //}
        #endregion

    }
}
