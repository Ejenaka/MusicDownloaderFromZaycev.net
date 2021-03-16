using System.Threading.Tasks;
using HtmlAgilityPack;
using MusicDownloader.Core.ParserCore;

namespace MusicDownloader.Core
{
    class HtmlLoader
    {
        readonly HtmlWeb client;
        readonly string url;

        public HtmlLoader(ParserSettings settings)
        {
            client = new HtmlWeb();
            url = $"{settings.BaseUrl}/{settings.Prefix}";
        }

        public async Task<HtmlDocument> LoadDocumentAsync(string searchQuery)
        {
            var currentUrl = url.Replace("{SearchQuery}", searchQuery).Replace("{PageNum}", "1");
            var responce = await client.LoadFromWebAsync(currentUrl);
            return responce;
        }
    }
}
