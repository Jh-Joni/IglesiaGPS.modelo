using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Iglesia.Api.consumer
{
    public static class Crud<T>
    {
        public static string EndPoint { get; set; } = string.Empty;

        // HttpClient reutilizable con timeout largo (Render Free tarda en despertar)
        private static readonly HttpClient _client = CreateClient();

        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            // Aceptar certificados SSL en contenedores Linux
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(120); // 2 min para Render Free cold start
            return client;
        }

        public static List<T> GetAll()
        {
            var response = _client.GetAsync(EndPoint).Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Error {response.StatusCode}: {errorResponse}");
            }
        }

        public static T GetById(int id)
        {
            var response = _client.GetAsync($"{EndPoint}/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json)!;
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Error {response.StatusCode}: {errorResponse}");
            }
        }

        public static T Create(T item)
        {
            var response = _client.PostAsync(
                    EndPoint,
                    new StringContent(
                        JsonConvert.SerializeObject(item),
                        Encoding.UTF8,
                        "application/json"
                    )
                ).Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json)!;
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Error {response.StatusCode}: {errorResponse}");
            }
        }

        public static bool Update(int id, T item)
        {
            var response = _client.PutAsync(
                    $"{EndPoint}/{id}",
                    new StringContent(
                        JsonConvert.SerializeObject(item),
                        Encoding.UTF8,
                        "application/json"
                    )
                ).Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Error {response.StatusCode}: {errorResponse}");
            }
        }

        public static bool Delete(int id)
        {
            var response = _client.DeleteAsync($"{EndPoint}/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Error {response.StatusCode}: {errorResponse}");
            }
        }

        public static T GetSingle(string customUrl)
        {
            var response = _client.GetAsync(customUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json)!;
            }
            return default!;
        }

        public static List<T> GetCustom(string customUrl)
        {
            var response = _client.GetAsync(customUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Error {response.StatusCode}: {errorResponse}");
            }
        }

        public static bool PutCustom(string customUrl)
        {
            var response = _client.PutAsync(customUrl, null).Result;
            return response.IsSuccessStatusCode;
        }

    }
}
