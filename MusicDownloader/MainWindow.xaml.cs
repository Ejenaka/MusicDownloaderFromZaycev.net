using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MusicDownloader.Core.ParserCore;
using MusicDownloader.Core;
using System.Net;
using System.ComponentModel;
using System.IO;

namespace MusicDownloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HtmlLoader loader;
        Parser parser;
        List<Song> songs;
        List<Label> songNameLabels;
        List<Label> songDurationLabels;
        List<Button> songDownloadButtons;
        readonly WebClient client;
        readonly string downloadPath = @"C:\Users\User\Music\";
        int currentSongsPage = 1;

        /// <summary>
        /// Clears songs grid on MainWindow
        /// </summary>
        private void ClearSongBox()
        {
            SongsBoxContainer.Children.Clear();
            errorMessage.Visibility = Visibility.Hidden;
        }

        private void CheckForDownloadedSongs()
        {
            if (songs == null)
                return;

            foreach (var songLabel in songNameLabels)
            {
                if (File.Exists(downloadPath + songLabel.Content + ".mp3"))
                {
                    songLabel.Foreground = Brushes.Gray;
                }
            }     
        }

        private void showSongsOnWindow(int songsPageNum)
        {
            ClearSongBox();

            for (int i = songsPageNum * 10 - 10, j = 0; i < songs.Count; i++, j++)
            {
                if (i >= songsPageNum * 10)
                    break;

                Grid.SetRow(songNameLabels[i], j);
                Grid.SetRow(songDurationLabels[i], j);
                Grid.SetRow(songDownloadButtons[i], j);
                Grid.SetColumn(songNameLabels[i], 0);
                Grid.SetColumn(songDurationLabels[i], 1);
                Grid.SetColumn(songDownloadButtons[i], 3);
                SongsBoxContainer.Children.Add(songNameLabels[i]);
                SongsBoxContainer.Children.Add(songDurationLabels[i]);
                SongsBoxContainer.Children.Add(songDownloadButtons[i]);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            client = new WebClient();
        }

        /// <summary>
        /// Event handler of the search button for showing list of songs and manupulating buttons on MainWindow
        /// </summary>
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSongBox();
            SwitchingButtons.Visibility = Visibility.Visible;
            string querySongName = textBox.Text;

            loader = new HtmlLoader(new ParserSettings());
            parser = new Parser();

            try
            {
                songs = parser.Parse(await loader.LoadDocumentAsync(querySongName)).ToList();
            }
            catch (ArgumentNullException)
            {
                errorMessage.Visibility = Visibility.Visible;
                return;
            }

            songNameLabels = songs.Select(song => new Label 
            { 
                Content = song.Name,
                FontFamily = new FontFamily("Microsoft YaHei UI Light")
            }).ToList();

            songDurationLabels = songs.Select(song => new Label 
            { 
                Content = song.Duration,
                FontFamily = new FontFamily("Microsoft YaHei UI Light")
            }).ToList();

            songDownloadButtons = songs.Select(song => new Button
            {
                Content = "Скачать",
                Tag = song.DownloadLink + "_%_" + song.Name + ".mp3",
                FontFamily = new FontFamily("Microsoft YaHei UI Light")
            }).ToList();

            CheckForDownloadedSongs();
            showSongsOnWindow(currentSongsPage);
        }

        private void DownloadSong(object sender, RoutedEventArgs e)
        {
            string[] songInfo = ((Button)e.OriginalSource).Tag.ToString().Split(new string[]{"_%_"}, StringSplitOptions.RemoveEmptyEntries);
            string downloadLink = songInfo[0];
            string songName = songInfo[1];
            System.Diagnostics.Debug.WriteLine(downloadLink + " " + songName);
            client.DownloadFileAsync(new Uri(downloadLink), downloadPath+songName);
            ProgressDownloadPanel.Visibility = Visibility.Visible;
            ProgressLabel.Content += songName;
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFinished);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressView);
        }

        private void previousPage_click(object sender, RoutedEventArgs e)
        {
            if (currentSongsPage == 1)
                return;

            showSongsOnWindow(--currentSongsPage);
        }

        private void nextPage_click(object sender, RoutedEventArgs e)
        {
            if (songs.Count < currentSongsPage * 10)
                return;

            showSongsOnWindow(++currentSongsPage);
        }

        private void DownloadProgressView(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressDownloadBar.Value = e.ProgressPercentage;
        }

        private void DownloadFinished(object sender, AsyncCompletedEventArgs e)
        {
            ProgressDownloadBar.Value = 0;
            ProgressDownloadPanel.Visibility = Visibility.Hidden;
        }
        
    }
}
