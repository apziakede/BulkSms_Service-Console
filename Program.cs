using BulkSms_Service.Models;
using BulkSms_Service.Services;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

        var excelFilePath = args.Length > 0 ? args[0] : "recipients.xlsx";

        if (!File.Exists(excelFilePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: File not found: {excelFilePath}");
            Console.ResetColor();
            return;
        }

        var config = new BulkSmsConfig
        {
            ExcelFilePath = excelFilePath,
            ApiKey = "SMTPUBK_d059396db7f848c18ce3a2fe481e4d65",
            SenderId = "PRISMS"
        };

        try
        {
            var smsService = new SmsService(config);
            var bulkSender = new ExcelBulkSmsSender(smsService, config);

           var response = await bulkSender.ProcessAndSendAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(response);
            Console.WriteLine("Done!");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unhandled error: {ex.Message}");
            Console.WriteLine(ex);
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
