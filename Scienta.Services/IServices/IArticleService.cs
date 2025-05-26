using Scienta.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scienta.Services.IServices
{
    public interface IArticleService
    {
        Task<List<ArticleModel>> GetPopSciArticles(int Id);
        Task<List<ArticleModel>> GetEvrimAgaciArticles(int Id);
        Task<List<ArticleModel>> GetBilimFiliArticles(int Id);
        Task<ReadArticleModel> GetArticleFromPopSci(string querystring);

    }
}
