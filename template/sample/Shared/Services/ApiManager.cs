using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace ChatWithDocsMobileApp.Shared.Services
{
    public class ApiManager
    {
        private readonly HttpClient _httpClient;

        public ApiManager()
        {
            // Initialize HttpClient (you can configure it further as needed)
            _httpClient = new HttpClient();
        }

        public enum RequestMethod
        {
            GET,
            POST,
            PUT,
            DELETE
        }

      
        public async Task<string> SendRequestAsync(
            string apiPath,
            RequestMethod method,
            object data = null,
            Dictionary<string, string> headers = null)
        {
            try
            {
                var url = $"{ConfigurationManager.URLString}{apiPath}";
                // Configure HttpRequestMessage
                using var requestMessage = new HttpRequestMessage
                {
                    Method = ConvertToHttpMethod(method),
                    RequestUri = new Uri(url)
                };
                // Add payload if the method supports it
                if (data != null && (method == RequestMethod.POST || method == RequestMethod.PUT))
                {
                    string jSonData = JsonSerializer.Serialize(data);
                     requestMessage.Content = new StringContent(jSonData, Encoding.UTF8, "application/json");
                }
               
                // Add headers if provided
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                // Send the request and get the response
                using var response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Read the error content as a string
                 var errorContent = await response.Content.ReadAsStringAsync();
                 var json = JsonSerializer.Deserialize<object>(errorContent);
                    
                  return $"{json}";
                  
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, rethrowing, etc.)
                Console.WriteLine(ex);
                throw new Exception($"Error during API call: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts RequestMethod to HttpMethod.
        /// </summary>
        private HttpMethod ConvertToHttpMethod(RequestMethod method)
        {
            return method switch
            {
                RequestMethod.GET => HttpMethod.Get,
                RequestMethod.POST => HttpMethod.Post,
                RequestMethod.PUT => HttpMethod.Put,
                RequestMethod.DELETE => HttpMethod.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(method), $"Unsupported HTTP method: {method}")
            };
        }
    }
}
