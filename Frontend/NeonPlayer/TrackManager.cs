using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeonPlayer
{
    internal class TrackManager
    {
        private MediaPlayer _player;
        string currentPlayedTrack = "";
        public TrackManager(MediaPlayer mp)
        {
            _player = mp;
        }

        public void DownloadTrack(string trackName)
        {
            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync("https://www.boggy.dev/neon/" + trackName + ".mp3"))
                {
                    System.IO.Directory.CreateDirectory("neon");
                    using (var fs = new FileStream("neon/" + trackName + ".mp3", FileMode.OpenOrCreate))
                    {
                        s.Result.CopyTo(fs);
                    }
                    PlayTrack(trackName);
                }
            }
        }

        public void PlayTrack(string trackName)
        {
            PauseTrack();
            _player.Open(new Uri("neon/" + trackName + ".mp3", UriKind.Relative));

            ReplaceOldTrack(trackName);

            _player.Play();
        }

        public void PauseTrack()
        {
            _player.Pause();
        }

        public void ResumeTrack()
        {
            _player.Play();
        }

        public void SeekTrack(double progress) // 0.0 to 1.0
        {
            _player.Position = new TimeSpan((long)(progress * _player.NaturalDuration.TimeSpan.Ticks));
        }

        public void ReplaceOldTrack(string newName)
        {
            if (File.Exists("neon/" + currentPlayedTrack + ".mp3"))
            {
                // If file found, delete it
                File.Delete("neon/" + currentPlayedTrack + ".mp3");
            }
            currentPlayedTrack = newName;
        }
    }
}
