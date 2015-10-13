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
            foreach (SyndicationItem item in feed.Items)
            {
                string title = item.Title.Text;
                DateTime date = item.PublishDate.DateTime;
                if (date.AddHours(HoursToAdd) > StartTime)
                {
                    Console.WriteLine("{0}, {1}", title, date.ToString());
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
            TimerCallback tcb = GetRssFromTutBy;
            Timer stateTimer = new Timer(tcb, null, 0, 6000);
            Console.ReadLine();
        }
    }
}
