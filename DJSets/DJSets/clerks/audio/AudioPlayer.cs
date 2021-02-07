using System;
using System.Windows.Media;

namespace DJSets.clerks.audio
{
    /// <summary>
    /// This class is an own implementation of MediaPlayer and defines extra fields and functions
    /// by using composition over inheritance
    /// </summary>
    public class AudioPlayer
    {
        #region Constructors
        public AudioPlayer()
        {
            RegisterEvents();
        }

        #endregion

        #region Fields
        /// <summary>
        /// This field provides the actual audioplayer functionality
        /// </summary>
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();

        /// <summary>
        /// This field indicates whether the AudioPlayer is playing or not
        /// </summary>
        private bool _isPlaying;

        /// <summary>
        /// This field exposes IsPlaying to outside
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                if (value == _isPlaying)
                {
                    return;
                }
                
                _isPlaying = value;

                var eventHandler = _isPlaying ? OnPlay : OnStop;
                eventHandler?.Invoke(this, new AudioPlayerPlayEventArgs() { PlayingPath = _mediaPlayer.Source.LocalPath });
            }
        }

        /// <summary>
        /// This field exposes the source of <see cref="_mediaPlayer"/>
        /// </summary>
        public Uri Source => _mediaPlayer.Source;
        #endregion

        #region Events
        /// <summary>
        /// This Event is called when the Media played by <see cref="_mediaPlayer"/> ended
        /// </summary>
        public event EventHandler OnMediaEnded;

        /// <summary>
        /// This event is called when <see cref="_mediaPlayer"/> starts Playing
        /// </summary>
        public event EventHandler<AudioPlayerPlayEventArgs> OnPlay;

        /// <summary>
        /// This event is called when <see cref="_mediaPlayer"/> stops playing
        /// </summary>
        public event EventHandler<AudioPlayerPlayEventArgs> OnStop;

        #endregion

        #region Functions
        /// <summary>
        /// This function plays <see cref="_mediaPlayer"/>
        /// </summary>
        public void Play()
        {
            _mediaPlayer.Play();
            IsPlaying = true;
        }

        /// <summary>
        /// This function stops <see cref="_mediaPlayer"/>
        /// </summary>
        public void Stop()
        {
            _mediaPlayer.Stop();
            IsPlaying = false;
        }

        /// <summary>
        /// This function pauses <see cref="_mediaPlayer"/>
        /// </summary>
        public void Pause()
        {
            _mediaPlayer.Pause();
            IsPlaying = false;
        }

        /// <summary>
        /// This function opens <see cref="_mediaPlayer"/>
        /// </summary>
        public void Open(Uri source)
        {
            _mediaPlayer.Open(source);
            IsPlaying = false;
        }

        /// <summary>
        /// This function closes <see cref="_mediaPlayer"/>
        /// </summary>
        public void Close()
        {
            _mediaPlayer.Close();
            IsPlaying = false;
        }
        #endregion

        #region Help Functions
        /// <summary>
        /// This function registers events to <see cref="_mediaPlayer"/>
        /// </summary>
        private void RegisterEvents()
        {
            _mediaPlayer.MediaEnded += delegate (object sender, EventArgs args)
            {
                IsPlaying = false;
                OnMediaEnded?.Invoke(sender, args);
            };
        }

        #endregion
    }

    /// <summary>
    /// This class defines EventArgs for the Playing-Events of the AudioPlayer
    /// </summary>
    public class AudioPlayerPlayEventArgs : EventArgs
    {
        /// <summary>
        /// This field contains the current FilePath to the Audio that is currently played
        /// </summary>
        public string PlayingPath { get; set; }
    }
}
