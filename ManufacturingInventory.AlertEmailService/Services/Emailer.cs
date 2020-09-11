using ManufacturingInventory.Domain.DTOs;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace ManufacturingInventory.AlertEmailService.Services {

    public class EmailRecipient {
        public string EmailAddress { get; set; }
        public string Message { get;private set; }
        private DateTime _dateTime;
        public bool IsFinalized { get; private set; }
        public MessageBuilder MessageBuilder { get; private set; }
        public List<AlertDto> Alerts { get; set; }

        public EmailRecipient(string address) {
            this.IsFinalized = false;
            this.EmailAddress = address;
            this.MessageBuilder = new MessageBuilder();
            this.MessageBuilder.StartMessage();
        }

        public async Task BuildAlertTable() {

        }

        public void FinalizeMessage() {
            this.Message=this.MessageBuilder.FinishMessage();
            this.IsFinalized = true;
        }

    }

    public class Emailer {
        //public static readonly ExchangeVersion ExchangeVersion = ExchangeVersion.Exchange2007_SP1;
        //public static readonly WebCredentials Credentials = new WebCredentials("inventoryalerts", "!23seti", "sskep.com");
        //public static readonly string From = "inventoryalerts@s-et.com";
        private ExchangeService _exchange;

        public Emailer() {
            this._exchange = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            WebCredentials credentials = new WebCredentials("inventoryalerts", "!23seti", "sskep.com");
            this._exchange.Credentials = credentials;
            this._exchange.Url = new Uri(@"https://email.seoulsemicon.com/EWS/Exchange.asmx");
        }

        public async Task SendMessageAsync(EmailRecipient emailRecipient) {
            if (!emailRecipient.IsFinalized)
                emailRecipient.FinalizeMessage();

            EmailMessage message = new EmailMessage(this._exchange);
            message.ToRecipients.Add(emailRecipient.EmailAddress);
            message.Subject = "Manufacturing Inventory Alert Service";
            MessageBody body = new MessageBody();
            body.BodyType = BodyType.HTML;
            body.Text =emailRecipient.Message;
            message.Body = body;
            await message.SendAndSaveCopy();
        }
    }
}
