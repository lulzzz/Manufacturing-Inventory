using System;
using System.Collections.Generic;
using System.Text;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.AlertEmailService.Services {
    public interface IMessageBuilder {
        void StartMessage();
        void AppendAlert(AlertDto alert);
        string FinishMessage();
    }


    public class MessageBuilder:IMessageBuilder {
        private StringBuilder _alertBuilder;

        public MessageBuilder() {
            this._alertBuilder = new StringBuilder();
        }
  
        public string FinishMessage() {
            this._alertBuilder.AppendLine("</table>");
            this._alertBuilder.AppendLine("<h2>----End Alerts----</h2>");
            this._alertBuilder.AppendLine("</body>");
            return this._alertBuilder.ToString();
        }

        public void StartMessage() {
            this._alertBuilder.AppendLine("<head>");
            this._alertBuilder.AppendLine("<style>");
            this._alertBuilder.AppendLine("table, th, td {");
            this._alertBuilder.AppendLine("border: 1px solid black;");
            this._alertBuilder.AppendLine("border-collapse: collapse;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("th, td {");
            this._alertBuilder.AppendLine("padding: 15px;");
            this._alertBuilder.AppendLine("text-align:center;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("table#t01 tr:nth-child(even) {");
            this._alertBuilder.AppendLine("background-color: #eee;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("table#t01 tr:nth-child(odd) {");
            this._alertBuilder.AppendLine("background-color: #B4AFAF;");
            this._alertBuilder.AppendLine("}");

            this._alertBuilder.AppendLine("table#t01 th{");
            this._alertBuilder.AppendLine("background-color: #A01C00;");
            this._alertBuilder.AppendLine("color:white;");
            this._alertBuilder.AppendLine("}");
            this._alertBuilder.AppendLine("</style>");
            this._alertBuilder.AppendLine("</head>");

            this._alertBuilder.AppendLine("<body>");
            this._alertBuilder.AppendLine("<h2>Manufacturing Inventory Alerts</h2>");
            this._alertBuilder.AppendLine("<table id=\"t01\" style=\"width:50%\">");
            this._alertBuilder.AppendLine("<tr>");
                this._alertBuilder.AppendLine("<th>Alert</th>");
                this._alertBuilder.AppendLine("<th>AlertType</th>");
                this._alertBuilder.AppendLine("<th>AlertStatus</th>");
                this._alertBuilder.AppendLine("<th>Quantity</th>");
                this._alertBuilder.AppendLine("<th>Min Stock</th>");
                this._alertBuilder.AppendLine("<th>Safe Stock</th>");
            this._alertBuilder.AppendLine("</tr>");
        }

        public void AppendAlert(AlertDto alert) {
            string alertStatus = "";
            switch (alert.AlertStatus) {
                case AlertStatus.StockAlert:
                    alertStatus = "Stock Alarm";
                    break;
                case AlertStatus.StockWarning:
                    alertStatus = "Stock Warning";
                    break;
                case AlertStatus.StockNoAlert:
                    alertStatus = "Stock Okay";
                    break;
                default:
                    alertStatus = "";
                    break;
            }
            this._alertBuilder.AppendLine("<tr>");
                this._alertBuilder.AppendFormat("<td>{0}</td>", alert.AlertIdentifier).AppendLine();
                this._alertBuilder.AppendFormat("<td>{0}</td>", alert.AlertType).AppendLine();
                this._alertBuilder.AppendFormat("<td>{0}</td>", alertStatus).AppendLine();
                this._alertBuilder.AppendFormat("<td>{0}</td>", alert.Quantity).AppendLine();
                this._alertBuilder.AppendFormat("<td>{0}</td>", alert.MinQuantity).AppendLine();
                this._alertBuilder.AppendFormat("<td>{0}</td>", alert.SafeQuantity).AppendLine();
            this._alertBuilder.AppendLine("</tr>");
        }

    }
}
