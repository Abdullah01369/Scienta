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
                       from="POPSCI"
                    });
                }
            }

            return links; 
        }

        public static string GenerateSlug(string title)
        {
            Dictionary<char, string> charMap = new()
    {
        {'ç', "c"}, {'Ç', "c"},
        {'ğ', "g"}, {'Ğ', "g"},
        {'ı', "i"}, {'İ', "i"},
        {'ö', "o"}, {'Ö', "o"},
        {'ş', "s"}, {'Ş', "s"},
        {'ü', "u"}, {'Ü', "u"}
    };

            title = title.ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (char c in title)
            {
                if (charMap.ContainsKey(c))
                    sb.Append(charMap[c]);
                else
                    sb.Append(c);
            }

            string slug = sb.ToString();

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // Alfasayısal dışı sil
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

            return slug;
        }

    }
}
