using BulkSms_Service.Constants;
using BulkSms_Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BulkSms_Service.Services
{
    public class SmsService
    {
        private readonly HttpClient _httpClient;
        private readonly BulkSmsConfig _config;

        public SmsService(BulkSmsConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _config.ApiKey);
        }

        public async Task<bool> SendSmsAsync(string recipient, string message)
        {
            var payload = new
            {
                sender = _config.SenderId,
                recipients = recipient,
                msg = message,
                type = SmsTemplates.Type,
                route = SmsTemplates.Route,
                ext_ref = Guid.NewGuid().ToString()
            };

            var response = await _httpClient.PostAsJsonAsync(_config.ApiUrl, payload);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to send to {recipient}: {response.StatusCode} - {body}");
                return false;
            }

            Console.WriteLine($"Sent to {recipient}: {body}");
            return true;
        }
    }
}
