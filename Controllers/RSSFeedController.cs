using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using WallaRSSMVC.Models;

namespace WallaRSSMVC.Controllers
{
    public class RSSFeedController : Controller
    {
        
      
        // GET: RSSFeed
        public ActionResult Index()
        {
            if (this.Session["_LastLoadedRss"] != null)
            {
                return Index(this.Session["_LastLoadedRss"].ToString());
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string RSSURL)
        {
            if (CheckForInternetConnection())
            {
                var model = ParseRss(RSSURL);
                HttpContext.Session.Add("_LastLoadedRss", RSSURL);
                return View(model);
            }
            else
            {
                var modelOffline = ParseRssOffline(RSSURL);

                return View(modelOffline);

            }
          
        }
        #region Private Methods
        private IEnumerable<RSSFeed> ParseRssOffline(string path)
        {

            path = path.Replace("http://rss.walla.co.il/feed/", "");
            string localPath = "";
            switch (path)
            {
                case "1?type=main":
                    localPath = Server.MapPath("~/Content/LocalXmls/wallaNews.xml");
                    this.Session["_LastLoadedRss"] = localPath;
                    return ParseLocalRss(localPath);
                case "3?type=main":
                    localPath = Server.MapPath("~/Content/LocalXmls/wallaSport.xml");
                    this.Session["_LastLoadedRss"] = localPath;
                    return ParseLocalRss(localPath);
                case "31?type=main":
                    localPath = Server.MapPath("~/Content/LocalXmls/wallaCars.xml");
                    this.Session["_LastLoadedRss"] = localPath;
                    return ParseLocalRss(localPath);
                case "24?type=main":
                    localPath = Server.MapPath("~/Content/LocalXmls/wallaFashion.xml");
                    this.Session["_LastLoadedRss"] = localPath;
                    return ParseLocalRss(localPath);
                default:
                    return null;
            }


        }

        private IEnumerable<RSSFeed> ParseLocalRss(string localPath)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(localPath);
                string xmlcontents = document.InnerXml;
                XDocument xml = XDocument.Parse(xmlcontents);
                var RSSFeedData = (from x in xml.Descendants("item")
                                   select new RSSFeed
                                   {
                                       Title = ((string)x.Element("title")),
                                       Link = ((string)x.Element("link")),
                                       Description = ((string)x.Element("description")),
                                       PubDate = ((DateTime)x.Element("pubDate")).ToString("dd.MM.yyyy HH:mm:ss")//.Replace(" +0200", "")
                                   });
                return RSSFeedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private IEnumerable<RSSFeed> ParseRss(string url)
        {
            try
            {

                WebClient wclient = new WebClient();

                wclient.Encoding = Encoding.UTF8;
                string RSSData = wclient.DownloadString(url);
                XDocument xml = XDocument.Parse(RSSData);
                var RSSFeedData = (from x in xml.Descendants("item")
                                   select new RSSFeed
                                   {
                                       Title = ((string)x.Element("title")),
                                       Link = ((string)x.Element("link")),
                                       Description = ((string)x.Element("description")),
                                       PubDate = ((DateTime)x.Element("pubDate")).ToString("dd.MM.yyyy HH:mm:ss")//.Replace(" +0200", "")
                                   });
                return RSSFeedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }

        private  void readFile(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            path = path.Replace("http://rss.walla.co.il/feed/", "");

            switch (path)
            {
                case "1?type=main":
                    document.Save(Path.Combine(
           Server.MapPath("~/Content/LocalXmls"), "wallaNews.xml"));
                    break;
                case "3?type=main":

                    document.Save(Path.Combine(
                   Server.MapPath("~/Content/LocalXmls"), "wallaSport.xml"));
                    break;
                case "31?type=main":
                    document.Save(Path.Combine(
                     Server.MapPath("~/Content/LocalXmls"), "wallaCars.xml"));
                    break;
                case "24?type=main":
                    document.Save(Path.Combine(
                     Server.MapPath("~/Content/LocalXmls"), "wallaFashion.xml"));
                    break;


            }
        }

        private bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}