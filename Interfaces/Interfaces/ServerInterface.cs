using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public abstract class ServerInterface
    {
        private static HttpClient Client;

        public static string BaseAddress()
        {
            return "https://localhost:7010";
        }

        private static HttpClient GetClient()
        {
            if (Client == null)
            {
                Client = new HttpClient();
            }
            if (string.IsNullOrWhiteSpace(Client.BaseAddress.ToString()))
            {
                Client.BaseAddress = new Uri(BaseAddress());
            }
            return Client;
        }

        public static async Task<T> GetAsync<T>(string subAddress)
        {
            var response = default(T);
            var httpResponse = await GetClient().GetAsync(Path.Combine(BaseAddress(), subAddress));
            if (httpResponse != null && httpResponse.IsSuccessStatusCode)
            {
                response = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
            }
            return response;
        }

        public static async Task<T> PostAsync<T>(string subAddress, object content)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(content));
            var response = default(T);
            var httpResponse = await GetClient().PostAsync(Path.Combine(BaseAddress(), subAddress), stringContent);
            if (httpResponse != null && httpResponse.IsSuccessStatusCode)
            {
                response = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
            }
            return response;
        }

        public static async Task<T> DeleteAsync<T>(string subAddress)
        {
            var response = default(T);
            var httpResponse = await GetClient().DeleteAsync(Path.Combine(BaseAddress(), subAddress));
            if (httpResponse != null && httpResponse.IsSuccessStatusCode)
            {
                response = JsonConvert.DeserializeObject<T>(await httpResponse.Content.ReadAsStringAsync());
            }
            return response;
        }
    }
}
