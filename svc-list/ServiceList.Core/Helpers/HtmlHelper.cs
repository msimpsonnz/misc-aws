using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ServiceList.Core
{
    public class HtmlHelper
    {
        public static async Task<List<Service>> ParseAwsServices(string url)
        {
            HtmlDocument pageDocument = await GetHtmlfromSite(url);

            var rawServices = pageDocument.DocumentNode.SelectNodes("(//div[contains(@class,'lb-content-item')]/*)");

            List<Service> serviceList = new List<Service>();
            foreach (HtmlNode item in rawServices)
            {
                var link = item.Attributes["href"].Value.Split('?')[0];
                Service service = new Service();
                service.id = HashHelper.HashString(link);
                service.ShortName = item.Attributes["href"].Value.Split('/')[1];
                service.Name = item.FirstChild.InnerText.Trim();
                service.Description = item.ChildNodes["span"].InnerText;
                service.Link = $"https://aws.amazon.com{link}";
                serviceList.Add(service);

            }
            
            return serviceList;
        }

        private static async Task<HtmlDocument> GetHtmlfromSite(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);
            return pageDocument;
        }

        public static async Task<List<Tags>> ParseBlogTags(string url)
        {
            HtmlDocument pageDocument = await GetHtmlfromSite(url);

            var rawTags = pageDocument.DocumentNode.SelectNodes("(//div[contains(@class,'tags')]/*)");

            List<Tags> tagList = new List<Tags>();
            foreach (HtmlNode item in rawTags)
            {
                var link = item.FirstChild.Attributes["href"].Value;
                foreach (var i in item.ChildNodes)
                {
                    if (i.Name == "ul")
                    {
                        Tags tags = new Tags();
                        tags.id = HashHelper.HashString(i.InnerText + link);
                        tags.Tag = i.InnerText;
                        tags.Blog = $"https://msimpson.co.nz{link}";
                        tagList.Add(tags);
                    }

                }
            }
            return tagList;
        }
    }
}