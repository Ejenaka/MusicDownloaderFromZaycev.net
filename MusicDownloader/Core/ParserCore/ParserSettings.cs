namespace MusicDownloader.Core.ParserCore
{
    class ParserSettings
    {
        public string BaseUrl { get; } = @"https://zaycev.net";
        public string Prefix { get; } = "search.html?spa=false&query_search={SearchQuery}&page={PageNum}";
    }
}
