using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scienta.Services.IServices;
using Scienta.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Scienta.Services.Services
{
    public class AiServices : IAiServices
    {
        private readonly string apiKey;

        private readonly IConfiguration _configuration;


        public AiServices(IConfiguration conf)
        {
            _configuration = conf;
            apiKey = _configuration.GetSection("geminikey").Value;
        }

        public async Task<ArticleSummaryResponse> GetSummaryFromDeepSeek(string inputText)
        {

            string promttext = "Senden bu bilimsel makalenin özetini çıkartmanı istiyorum. ek bilgiler de katabilirsin doğru olmak şartı ile. uzun bir özet olmasına gerek yok. varsa anahtar kelimeleri belirleyebilirsin." +
                " Cevap olarak sadece istediğim şeyi yaz, başka bir şey yazamana gerek yok,cevabını html olarak ver, p etiketi span vs kullanabilrsin,gerektiği yerde istediğin html etiketi kullanabilirsin," +
                " dogrudan bana cevabını ekrana basacağim bunu düşünerek uygun formatta gönder, İşte makale : " + inputText;
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://openrouter.ai/api/v1/chat/completions");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);



            var requestBody = new
            {
                //model = "google/gemini-2.5-flash-preview-05-20",
                model = "google/gemini-2.0-flash-001",
                messages = new[]
           {
                new { role = "user", content = promttext }
            }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Hata: " + responseBody);
                    return new ArticleSummaryResponse() { Error = responseBody, IsSuccess = false };
                }

                var result = JObject.Parse(responseBody);
                var messageContent = result["choices"]?[0]?["message"]?["content"]?.ToString();

                return new ArticleSummaryResponse() { Content = messageContent, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ArticleSummaryResponse() { IsSuccess = false,Error=ex.Message };

            }


        }
    }
}
