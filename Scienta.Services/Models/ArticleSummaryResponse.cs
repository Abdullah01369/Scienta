using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scienta.Services.Models
{
    public class ArticleSummaryResponse
    {
        public string Content { get; set; }
        public string Error { get; set; }
        public bool IsSuccess { get; set; }
    }
}
