using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeonPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer _player;
        TrackManager _tm;

        List<Song> _songList;
        int currentSong = 0;
        bool playing = false;

        public MainWindow()
        {
            InitializeComponent();
            _player = new MediaPlayer();
            _tm = new TrackManager(_player);
            _songList = new List<Song>();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            dispatcherTimer.Start();

            var dir = System.IO.Directory.CreateDirectory("neon");
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }


            SelectSong("a");
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (playing)
            {
                if (_songList.Count <= currentSong) return;
                if (_songList[currentSong] == null) return;
                
                songLbl.Content = _songList[currentSong].title + " (" + _songList[currentSong].album + ")";
                if (_player.NaturalDuration != Duration.Automatic && !dragStarted)
                {
                    timeLbl.Content = _player.Position.ToString().Split('.')[0] + " / " + _player.NaturalDuration.ToString().Split('.')[0];
                    seekSlide.Value = (double)_player.Position.TotalMilliseconds / (double)_player.NaturalDuration.TimeSpan.TotalMilliseconds;
                    if (seekSlide.Value >= 0.998)
                    {
                        currentSong = (currentSong + 1) % _songList.Count;
                        ChangeSong();
                    }
                }
            }
        }

        private void playPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (playing)
            {
                _tm.PauseTrack();
                playing = false;
            }
            else
            {
                _tm.ResumeTrack();
                playing = true;
            }
        }

        private void songBackBtn_Click(object sender, RoutedEventArgs e)
        {
            currentSong = (_songList.Count + currentSong - 1) % _songList.Count;
            ChangeSong();
        }

        private void songFwdBtn_Click(object sender, RoutedEventArgs e)
        {
            currentSong = (currentSong + 1) % _songList.Count;
            ChangeSong();
        }

        private void ChangeSong()
        {
            _player.Stop();
            _tm.DownloadTrack(_songList[currentSong].title);
        }

        bool sliding = false;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliding)
            {
                _tm.SeekTrack(seekSlide.Value);
            }
        }

        private bool dragStarted = false;

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _tm.SeekTrack(seekSlide.Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sliding = true;
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            sliding = false;
            //_tm.SeekTrack(seekSlide.Value);
        }

        bool startup = true;

        public async void SelectSong(string song)
        {
            string uri = "https://localhost:7215/SongEngine?startSong=" + song;

            HttpService httpService = new HttpService();

            string res = await httpService.GetAsync(uri);

            if (res != null)
            {
                _songList = JsonSerializer.Deserialize<List<Song>>(res)!;

                theList.Items.Clear();

                for (int i = 0; i < _songList.Count; i++)
                {
                    string trackInfo = _songList[i].title + "-" + _songList[i].album + ", genre: " + _songList[i].genre + ", mood: " + _songList[i].mood;
                    
                    theList.Items.Add(trackInfo);
                }

                currentSong = 0;
                if (!startup)
                {
                    _tm.DownloadTrack(_songList[0].title);
                    playing = true;
                }
                
                startup = false;
            }
        }

        public void FillChoices()
        {
            if (_songList == null) return;

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                string trackName = item.ToString().Split('-')[0].Split(':')[1].Substring(1);
                SelectSong(trackName);
            }
        }

        private void volSlide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //change da wolume. my final message goodbye
            if(_player != null)
                _player.Volume = volSlide.Value;
        }
    }
}
