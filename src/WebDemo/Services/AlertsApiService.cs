using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WebDemo.Models;

namespace WebDemo.Services
{
    public class AlertsApiService : IDisposable
    {
        private readonly HttpClient _client;
        private const string _apiEndpoint = "https://world2capture.global-mmk.com/alert_analytics";

        public AlertsApiService()
        {
            _client = new HttpClient();
        }


        public async Task<AlertResponse> GetCurrentAlertsAsync()
        {
            var response = await _client.GetAsync(_apiEndpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AlertResponse>(responseContent);
            }

            throw new Exception($"Error: {response.ReasonPhrase} [{response.StatusCode}]");
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}