using HtmlAgilityPack;
using Scienta.Services.IServices;
using Scienta.Services.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
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
        private readonly string _baseUrlpopsci = "https://popsci.com.tr/";
        private readonly string _baseUrlevrimagaci = "https://evrimagaci.org/kategori/doga-bilimleri-15";
        private readonly string _baseUrlbilimfili = "https://bilimfili.com/kategori/doga-bilimleri?page=";
        private readonly string _baseUrlbilimfilireq = "https://bilimfili.com";


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


                links.Add(new ArticleModel { date = tarih, from = PlatformsModel.GetEvrimagaci(), href = link, img = imgSrc, title = title });

            }





            return links;
        }

        public async Task<List<ArticleModel>> GetBilimFiliArticles(int Id)
        {
            var links = new List<ArticleModel>();

            var client = _httpClientFactory.CreateClient();
            _httpClient = client;

            var querystring = $"{_baseUrlbilimfili}{Id}";

            var response = await client.GetAsync(querystring);

            var val = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(val);


            var items = doc.DocumentNode.SelectNodes("//div[contains(@class, 'row') and contains(@class, 'mb-3_5')]");

            foreach (var item in items)
            {

                var titleNode = item.SelectSingleNode(".//h3[contains(@class,'title')]/a");
                string link = _baseUrlbilimfilireq + titleNode?.GetAttributeValue("href", "").Trim();


                string title = titleNode?.InnerText.Trim();


                var imgNode = item.SelectSingleNode(".//div[contains(@class,'list-img')]//img");
                string imgSrc = _baseUrlbilimfilireq + imgNode?.GetAttributeValue("src", "").Trim();


                var smallNode = item.SelectSingleNode(".//small[contains(@class,'author')]");

                var tarih = HtmlEntity.DeEntitize(smallNode.InnerText).Trim().Split('|')[1].Trim();


                links.Add(new ArticleModel { date = tarih, from = PlatformsModel.GetBilimFili(), href = link, img = imgSrc, title = title });

            }

            return links;
        }



        public async Task<ReadArticleModel> GetArticleFromPopSci(string querystring)
        {
            var client = _httpClientFactory.CreateClient();
            _httpClient = client;
            var response = await client.GetAsync($"{querystring}");

            var val = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(val);

            var datediv = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content-details-container')]");
            var timeNode = datediv.SelectSingleNode(".//time");
            var date = timeNode?.InnerText.Trim();
            var headerDiv = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'header-titles')]");
            var h1 = headerDiv.SelectSingleNode(".//h1[contains(@class, 'title')]");
            var title = h1.InnerText.Trim();  //feed-type-custom
            var htmlcontent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content ql-style clearfix')]").InnerHtml;

            var dochtml = new HtmlDocument();
            dochtml.LoadHtml(htmlcontent);

            var spans = dochtml.DocumentNode.SelectNodes("//span[contains(@class, 'open-noads-modal')]");
            if (spans != null)
            {
                foreach (var span in spans)
                {
                    span.Remove();
                }
            }
            var advspan = dochtml.DocumentNode.SelectNodes("//div[contains(@class, 'feed-type-custom')]");
            if (advspan != null)
            {
                foreach (var adv in advspan)
                {
                    adv.Remove();
                }
            }


            htmlcontent = dochtml.DocumentNode.OuterHtml;

            return new ReadArticleModel() { Content = htmlcontent, From = PlatformsModel.GetEvrimagaci(), Title = title, Date = date, SourceLink = PlatformsModel.GetEvrimagaciLink() };
        }




    }
}
