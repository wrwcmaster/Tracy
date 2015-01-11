using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tracy.DataModel;

namespace Tracy.ResourceSource.Dmhy
{
    public class DmhyHtmlParser
    {
        public static List<Resource> Parse(int pageIndex, string sourceName)
        {
            var matchedItems = new List<Resource>();
            WebClient client = new WebClient();

            string url = "http://share.dmhy.org/topics/list/sort_id/2/page/" + pageIndex.ToString();
            byte[] contentData = null;

            int retryCount = 3;
            while (retryCount > 0)
            {
                retryCount--;
                try
                {
                    Console.WriteLine("Retrieving url " + url);
                    contentData = client.DownloadData(url);
                    Console.WriteLine("Success");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to retrieve html page: " + url + ", error: " + ex.Message + " Will try again.");
                    System.Threading.Thread.Sleep(1000);
                }
            }

            string html = Encoding.UTF8.GetString(contentData);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode rootNode = doc.DocumentNode.SelectSingleNode("//table[@id='topic_list']/tbody");
            if (rootNode != null)
            {
                HtmlNodeCollection selectedNodes = rootNode.SelectNodes("tr");
                if (selectedNodes != null)
                {
                    //Item lists
                    foreach (HtmlNode node in selectedNodes)
                    {
                        try
                        {
                            HtmlNode publishDateNode = node.SelectSingleNode("td[1]/span");
                            string publishDateStr = publishDateNode.InnerText;
                            DateTime publishDate = DateTime.Parse(publishDateStr);
                            publishDate = TimeZoneInfo.ConvertTime(publishDate, TimeZoneInfo.Utc); //Convert to utc time

                            HtmlNode titleNode = node.SelectSingleNode("td[@class='title']/a");
                            string title = titleNode.InnerText.Trim();

                            HtmlNode linkNode = node.SelectSingleNode("td/a[@class='download-arrow arrow-torrent']");
                            string link = "http://share.dmhy.org" + linkNode.Attributes["href"].Value;
                            matchedItems.Add(new Resource() { Title = title, Link = link, PublishDate = publishDate, Type = "Torrent", Source = sourceName, Status = 0 });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            return matchedItems;
        }
    }
}
