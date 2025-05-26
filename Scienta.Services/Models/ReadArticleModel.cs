using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scienta.Services.Models
{
    public class ReadArticleModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public string SourceLink { get; set; }

    }
}
