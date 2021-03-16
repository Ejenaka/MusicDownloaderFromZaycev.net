using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace MusicDownloader.Core.ParserCore
{
    class Parser
    {
        ParserSettings settings = new ParserSettings();

        // Returns parced songs from HTML document
        public IEnumerable<Song> Parse(HtmlDocument document)
        {
            var songElements = document.DocumentNode.SelectNodes(@"//div[contains(@class, 'musicset-track clearfix ')]");
            if (songElements == null)
            {
                return null;
            }
            var songNameElements = document.DocumentNode.SelectNodes(@"//div[@class='musicset-track__fullname']");
            var songDurationElements = document.DocumentNode.SelectNodes(@"//div[@class='musicset-track__duration']");
            var songDownloadLinksElements = document.DocumentNode.SelectNodes(@"//a[@class='musicset-track__download-link track-geo__button']");

            // Get values of nodes
            var parsedSongsName = songNameElements.Select(node => node.InnerText).ToArray();
            var parsedSongsDuration = songDurationElements.Select(node => node.InnerText).ToArray();
            var parsedSongsDownloadLink = songDownloadLinksElements.Select(node => settings.BaseUrl + node.Attributes["href"].Value).ToArray();

            var parsedSongs = new List<Song>();
            for (int i = 0, j = 0; i < parsedSongsName.Length; i++)
            {
               if (parsedSongsDuration[i] != "00:30" 
                    && !songElements[i].LastChild.InnerHtml.Contains("musicset-track__download musicset-track__download--banned musicset-track__download--disabled"))
               {
                    parsedSongs.Add(new Song(parsedSongsName[i], parsedSongsDuration[i], parsedSongsDownloadLink[j]));
                    j++;
               }
            }

            return parsedSongs;
        }
    }
}
