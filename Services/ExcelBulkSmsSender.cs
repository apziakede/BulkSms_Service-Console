#nullable disable
using BulkSms_Service.Constants;
using BulkSms_Service.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkSms_Service.Services
{
    public class ExcelBulkSmsSender
    {
        private readonly SmsService _smsService;
        private readonly BulkSmsConfig _config;

        public ExcelBulkSmsSender(SmsService smsService, BulkSmsConfig config)
        {
            _smsService = smsService;
            _config = config;
        }

        public async Task<string> ProcessAndSendAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(_config.ExcelFilePath));
            var worksheet = package.Workbook.Worksheets[_config.SheetNumber];

            if (worksheet == null)
            {
                Console.WriteLine($"Sheet number {_config.SheetNumber} not found.");
                return null;
            }

            int startRow = _config.SheetHasColumnHeaders ? 2 : 1;
            int rowCount = worksheet.Dimension.End.Row;
            var recipients = new List<Recipient>();

            for (int row = startRow; row <= rowCount; row++)
            {
                var firstName = worksheet.Cells[row, _config.NameColumnNumber].Text;
                var phoneNumber = worksheet.Cells[row, _config.PhoneColumnNumber].Text;

                var phoneValidation = IsValidPhone(phoneNumber);
                if (phoneValidation.isValid)
                {
                    recipients.Add(new Recipient
                    {
                        Name = firstName,
                        PhoneNumber = phoneValidation.normalizedPhoneNumber
                    });
                }
            }
            var result = await SendBulkSmsAsync(recipients);
            return $"Total number of messages sent is: {result}";
        }

        public async Task<int> SendBulkSmsAsync(List<Recipient> recipients)
        {
            int sentMessageCount = 0;

            foreach (var recipient in recipients)
            {
                var message = string.Format(SmsTemplates.MessageTemplate, recipient.Name);
                var result = await _smsService.SendSmsAsync(recipient.PhoneNumber, message);

                if (result) sentMessageCount++;
            }
            return sentMessageCount;
        }

        private (bool isValid, string normalizedPhoneNumber) IsValidPhone(string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                // Remove leading spaces and normalize
                string normalized = phoneNumber.Trim();

                // Remove "+234" or "234" if present
                if (normalized.StartsWith("+234"))
                    normalized = normalized.Substring(4);
                else if (normalized.StartsWith("234"))
                    normalized = normalized.Substring(3);

                // Remove leading zero if present
                if (normalized.StartsWith('0'))
                    normalized = normalized.Substring(1);

                // Check if remaining is all digits and has correct length (should be 10 digits for Nigerian numbers)
                if (normalized.Length == 10 && normalized.All(char.IsDigit))
                {
                    return (true, $"234{normalized}");
                }
                else return (false, null);
            }
            else return (false, null);
        }
    }
}