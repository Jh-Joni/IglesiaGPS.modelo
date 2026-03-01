using System.Text;
using Newtonsoft.Json;

namespace Iglesia.Api.consumer
{
    public static class Crud<T>
    {
        public static string EndPoint { get; set; } = string.Empty;

        public static List<T> GetAll()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(EndPoint).Result;
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
        }

        public static T GetById(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"{EndPoint}/{id}").Result;
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
        }

        public static T Create(T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
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
        }

        public static bool Update(int id, T item)
        {
            using (var client = new HttpClient())
            {
                var response = client.PutAsync(
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
        }

        public static bool Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync($"{EndPoint}/{id}").Result;
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
        }

        public static T GetSingle(string customUrl)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(customUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(json)!;
                }
                return default!;
            }
        }

        public static List<T> GetCustom(string customUrl)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(customUrl).Result;

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
        }

        public static bool PutCustom(string customUrl)
        {
            using (var client = new HttpClient())
            {
                var response = client.PutAsync(customUrl, null).Result;
                return response.IsSuccessStatusCode;
            }
        }

    }
}
