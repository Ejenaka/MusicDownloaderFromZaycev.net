namespace MusicDownloader.Core
{
    class Song
    {
        public string Name { get; set; }
        public string Duration { get; set; }
        public string DownloadLink { get; set; }

        public Song(string name, string duration, string downloadLink)
        {
            Name = name;
            Duration = duration;
            DownloadLink = downloadLink;
        }
    }
}
