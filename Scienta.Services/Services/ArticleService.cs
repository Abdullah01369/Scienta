using HtmlAgilityPack;
using Scienta.Services.IServices;
using Scienta.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scienta.Services.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static HttpClient _httpClient;
        private readonly string _baseUrl = "https://popsci.com.tr/kategori/konular/bilim";
        private readonly string _baseUrlevrimagaci = "https://evrimagaci.org/kategori/doga-bilimleri-15";

        //https://popsci.com.tr/kategori/konular/bilim/page/2/
        public ArticleService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;
        }


        public async Task<List<ArticleModel>> GetPopSciArticles(int Id)
        {
            var links = new List<ArticleModel>();

            var client = _httpClientFactory.CreateClient();
            _httpClient = client;
            var response = await client.GetAsync($"{_baseUrl}/page/{Id}/");

            var val = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(val);

            var aTags = doc.DocumentNode.SelectNodes("//div[contains(@class,'td-module-thumb')]/a");
            if (aTags != null)
            {
                foreach (var aTag in aTags)
                {
                    var href = aTag.GetAttributeValue("href", "");
                    var title = aTag.GetAttributeValue("title", "");

                    var imgNode = aTag.SelectSingleNode(".//img");
                    var imgUrl = imgNode?.GetAttributeValue("data-img-url", "");

                    links.Add(new ArticleModel
                    {
                        title = title,
                        href = href,
                        img = imgUrl,
                        from = PlatformsModel.GetPopsci()
                    });
                }
            }

            return links;
        }

        public async Task<List<ArticleModel>> GetEvrimAgaciArticles(int Id)
        {
            var links = new List<ArticleModel>();

            var client = _httpClientFactory.CreateClient();


            var query = (Id - 1) * 36;
            var querystring = $"{_baseUrlevrimagaci}/{query}";
            var response = await client.GetAsync(querystring);

            var val = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(val);


            var items = doc.DocumentNode.SelectNodes("//div[contains(@class, 'home-content-box-item')]");



            foreach (var item in items)
            {

                var titleNode = item.SelectSingleNode(".//h3[contains(@class,'home-content-box-title')]/a");
                string link = titleNode?.GetAttributeValue("href", "").Trim();
                string title = titleNode?.InnerText.Trim();


                var imgNode = item.SelectSingleNode(".//div[contains(@class,'position-relative')]//img");
                string imgSrc = imgNode?.GetAttributeValue("src", "").Trim();


                var tarihNode = item.SelectNodes(".//div[contains(@class,'home-content-box-tag')]")
                                 ?.FirstOrDefault(x => x.InnerText.Contains("Tarih"));
                string tarih = tarihNode?.InnerText.Replace("Tarih :", "").Trim();


                links.Add(new ArticleModel { date = tarih, from =PlatformsModel.GetEvrimagaci(), href = link, img = imgSrc, title = title });

            }





            return links;
        }

    }
}
