using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using HtmlAgilityPack;
using NewsExtractor.Models;
using Microsoft.EntityFrameworkCore;

namespace NewsExtractor.Services
{
    public class NewsExtractorClass : INewsExtractor
    {
        private readonly RefreshingUnitContext _context;
        public NewsExtractorClass(RefreshingUnitContext context)
        {
            _context = context;
        }

        public async Task<List<InfoTable>> ExtractAllInfo()
        {
            var result = _context.InfoTables.ToList();
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<PostMessage> InfoSender(InfoTable info)
        {
            PostMessage postMessage = new();
            if (info.Status == true)
            {
                _context.InfoTables.Add(info);
                _context.SaveChanges();
            }
            postMessage.Status = false;
            return null;
        }
        public async Task<InfoTable> InfoReturner()
        {
            using (WebClient wc = new())
            {
                InfoTable infoT = new();
                //CheckCall
                var callUrl = _context.RefreshingLinks.FirstOrDefault();
                var response = wc.DownloadString(callUrl.Link);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response);
                var extractedUrl = doc.DocumentNode.SelectSingleNode("//div[@class='newsfeed-wrapper']/div/span/a").Attributes["href"].Value;
                if (callUrl.Link == extractedUrl)
                {
                    infoT.Status = false;
                    return infoT;
                }
                else
                {
                    callUrl.Link = extractedUrl;
                    _context.RefreshingLinks.Update(callUrl);
                    _context.SaveChanges();
                    //DataCall
                    response = wc.DownloadString(extractedUrl);
                    doc.LoadHtml(response);

                    var extractedTitle = doc.DocumentNode.SelectSingleNode("//span[@class='entry-title']").InnerText;
                    infoT.Hours = RemoveWhitespacesUsingRegex(extractedTitle).Remove(5);
                    var date = doc.DocumentNode.SelectSingleNode("//span[@class='entry-date']").InnerText.Remove(19).Remove(0, 1);
                    infoT.Date = date;
                    var title = doc.DocumentNode.SelectSingleNode("//h1[@class='single-title']").InnerText.Remove(0, 29);
                    infoT.Title = title;
                    var generalInfo = doc.DocumentNode.SelectSingleNode("//div[@class='single-content-wrapper']").InnerText.Remove(0, 33);
                    infoT.AllInfo = generalInfo;
                    var descriptionUrl = doc.DocumentNode.SelectSingleNode("//div[@class='cat_icon']/a").Attributes["href"].Value;
                    infoT.SectionName = descriptionUrl.Remove(0, 23);
                    if (doc.DocumentNode.SelectSingleNode("//div[@class='single-image-wrapper']/img").Attributes["src"].Value == null)
                    {
                        response = wc.DownloadString(descriptionUrl);
                        doc.LoadHtml(response);
                        var imageUrl = doc.DocumentNode.SelectSingleNode("//related-img']/img").Attributes["src"].Value;
                        var image64String = WebUtility.UrlEncode(await ImageUrlToBase64(imageUrl));
                        infoT.ImageBytes = image64String;
                    }
                    else
                    {
                        var imageUrl = doc.DocumentNode.SelectSingleNode("//div[@class='single-image-wrapper']/img").Attributes["src"].Value;
                        var image64String = WebUtility.UrlEncode(await ImageUrlToBase64(imageUrl));
                        infoT.ImageBytes = image64String;
                    }
                    //DescriptionCall
                    response = wc.DownloadString(descriptionUrl);
                    var description = doc.DocumentNode.SelectSingleNode("//div[@class='related-excerpt']").InnerText.Remove(0, 33);
                    infoT.Description = description;
                    return infoT;
                }
            }
        }
        static async Task<string> ImageUrlToBase64(string imageUrl)
        {
            using var httClient = new HttpClient();
            var imageBytes = await httClient.GetByteArrayAsync(imageUrl);
            return Convert.ToBase64String(imageBytes);
        }
        static string RemoveWhitespacesUsingRegex(string source)
        {
            return Regex.Replace(source, @"\s", string.Empty);
        }
    }
}
