using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    internal class WebPage
    {
        public WebPage() { }

        public WebPage(string url, string email, string pass)
        {
            Url = url;
            this.email = email;
            this.pass = pass;
        }

        public string Url { get; set; }

        public string email { get; set; }

        public string pass { get; set; }

        public string tvShow { get; set; }

        public string playlist { get; set; }
    }
}
