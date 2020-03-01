using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get current User
            var response = GetCurrentUser().Result;

            if(response.IsSuccessStatusCode)
            {
                JObject user = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                //Get current System User
                response = GetCurrentSystemUser(user["UserId"].ToString()).Result;

                if (response.IsSuccessStatusCode)
                {
                    JObject systemuser = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(systemuser.ToString());
                }
                else
                {
                    Console.WriteLine("Request failed - '{0}'", response.ReasonPhrase);
                }
            }
            else
            {
                Console.WriteLine("Request failed - '{0}'", response.ReasonPhrase);
            }

            Console.ReadLine();
        }

        public static async Task<HttpResponseMessage> GetCurrentUser()
        {
            String accessToken = await GetAccessToken();

            var appSettings = ConfigurationManager.AppSettings;
            String apiUrl = appSettings["apiUrl"];

            HttpClient client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, apiUrl + "WhoAmI()");

            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=*");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(message);

            return response;
        }

        public static async Task<HttpResponseMessage> GetCurrentSystemUser(string UserId)
        {
            String accessToken = await GetAccessToken();

            var appSettings = ConfigurationManager.AppSettings;
            String apiUrl = appSettings["apiUrl"];

            HttpClient client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, apiUrl + $"systemusers({UserId})");

            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=*");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(message);

            return response;
        }

        private static async Task<string> GetAccessToken()
        {
            var appSettings = ConfigurationManager.AppSettings;

            String clientId = appSettings["clientId"];
            String secret = appSettings["secret"];
            String tenantId = appSettings["tenantId"];
            String resourceUrl = appSettings["resourceUrl"];

            var credentials = new ClientCredential(clientId, secret);
            var authContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenantId);
            var result = await authContext.AcquireTokenAsync(resourceUrl, credentials);

            return result.AccessToken;
        }
    }
}
