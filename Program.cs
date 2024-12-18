using System;
using System.Configuration;

namespace Application
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataLayer run = new DataLayer();

            if (ConfigurationManager.AppSettings["runHulu"].Equals("true"))
                run.RunHulu(ConfigurationManager.AppSettings["tvShow"]);


            if (ConfigurationManager.AppSettings["runSpotify"].Equals("true"))
                run.RunSpotify(Boolean.Parse( ConfigurationManager.AppSettings["Shuffle"] ), ConfigurationManager.AppSettings["SongSelection"]);
        }
    }
}
