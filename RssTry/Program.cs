using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;

namespace RssTry
{
    class Program
    {
        static public DateTime StartTime = DateTime.Now; 
        static public int HoursToAdd = 24;
        static public void GetRssFromTutBy(Object stateInfo)
        {
            Uri serviceUri = new Uri("http://news.tut.by/rss/index.rss");
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(downloader_OpenReadCompleted);
            downloader.OpenReadAsync(serviceUri);
        }

        static public void DisplayNewsItems(SyndicationFeed feed)
        {
            foreach (SyndicationItem item in feed.Items.Reverse())
            {
                string title = item.Title.Text;
                DateTime date = item.PublishDate.DateTime;
                DateTime dateWithAddHours = date.AddHours(HoursToAdd);
                if (dateWithAddHours > StartTime)
                {
                    Console.WriteLine();
                    Console.WriteLine(date.ToString());
                    Console.WriteLine(title);
                }
            }
            HoursToAdd = 0;
        }

        static void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Stream responseStream = e.Result;
                XmlReader responseReader = XmlReader.Create(responseStream);
                try 
                {
                    SyndicationFeed feed = SyndicationFeed.Load(responseReader);
                    DisplayNewsItems(feed);
                }
                catch (Exception error) 
                {
                    Console.WriteLine(error.Message);
                }
            }
        }
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("News of the last 24 hours from TUT.by\n");
                TimerCallback tcb = GetRssFromTutBy;
                Timer stateTimer = new Timer(tcb, null, 0, 60000);
                Console.ReadLine();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                Console.ReadLine();
            }
        }
    }
}
