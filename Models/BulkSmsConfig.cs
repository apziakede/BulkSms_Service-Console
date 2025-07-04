using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkSms_Service.Models
{
    public class BulkSmsConfig
    {
        public string ExcelFilePath { get; set; } = "recipients.xlsx";
        public int SheetNumber { get; set; } = 0;
        public bool SheetHasColumnHeaders { get; set; } = false;
        public int NameColumnNumber { get; set; } = 7;
        public int PhoneColumnNumber { get; set; } = 10;
        public string SenderId { get; set; } = "PRISMS";
        public string ApiUrl { get; set; } = "https://smsapi-sur2.onrender.com/api/sms/send";
        public string ApiKey { get; set; } = "SMTPUBK_d059396db7f848c18ce3a2fe481e4d65";
    }
}
