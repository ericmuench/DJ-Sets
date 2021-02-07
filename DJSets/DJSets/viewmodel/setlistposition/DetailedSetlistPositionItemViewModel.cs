using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DJSets.clerks.audio;
using DJSets.clerks.dataservices;
using DJSets.clerks.timeformat;
using DJSets.model.entityframework;
using DJSets.model.musickeys;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.setlistposition
{
    /// <summary>
    /// This class defines Viewmodel-logic for a detailed Song-View as an Item
    /// </summary>
    public class DetailedSetlistPositionItemViewModel : BaseViewModel, IElementContainer<SetlistPosition>
    {
        #region Constructors
        public DetailedSetlistPositionItemViewModel(SetlistPosition setlistPosition, 
            IDataService<SetlistPosition> dataService, AudioPlayer audioPlayer)
        {
            _setlistPosition = setlistPosition;
            _dataService = dataService;

            _audioPlayer = audioPlayer;
            _audioPlayer.OnStop += (sender, args) => OnPropertyChanged(nameof(PlayIcon));

            ConfigCommands();
        }
        #endregion

        #region Commands
        /// <summary>
        /// This command should remove this SetlistPosition from the Setlist
        /// </summary>
        public DelegateCommand<object,int> RemoveFromSetlistCommand { get; set; }

        /// <summary>
        /// This command interacts with audio --> when audio is playing stop audio and vice versa
        /// </summary>
        public DelegateCommand<object,bool> AudioInteractCommand { get; set; }

        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk transforms the duration value of the song of <see cref="_setlistPosition"/>
        /// into the correct format for beeing displayed
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();

        /// <summary>
        /// This DataService provides operations for interacting with the Model considering SetlistPositions
        /// </summary>
        private readonly IDataService<SetlistPosition> _dataService;

        /// <summary>
        /// This clerk allows playing audio
        /// </summary>
        private readonly AudioPlayer _audioPlayer;
        #endregion

        #region _setlistPosition

        /// <summary>
        /// This field guarantees access to the setlist position
        /// </summary>
        private readonly SetlistPosition _setlistPosition;

        #endregion

        #region SongTitle

        /// <summary>
        /// This field shows the title of the song of <see cref="_setlistPosition"/>
        /// </summary>
        public string SongTitle
        {
            get => _setlistPosition.Song?.Title ?? "No Title";
        }

        #endregion

        #region SongArtist

        /// <summary>
        /// This field shows the artist of the song of <see cref="_setlistPosition"/>
        /// </summary>
        public string SongArtist
        {
            get => _setlistPosition.Song?.Artist ?? "No Artist";
        }

        #endregion

        #region SongTempo

        /// <summary>
        /// This field shows the tempo of the song of <see cref="_setlistPosition"/>
        /// </summary>
        public string SongTempo
        {
            get
            {
                var bpm = _setlistPosition.Song?.Bpm ?? 0;
                return $"{bpm} {Application.Current.GetResource<string>(StringResourceKeys.StrBpm)}";
            }
        }

        #endregion

        #region MusicKey

        /// <summary>
        /// This field shows the MusicKey of the song of <see cref="_setlistPosition"/>
        /// </summary>
        public string SongMusicKey
        {
            get => _setlistPosition.Song?.MusicKey.GetTitle() 
                   ?? MusicKeysUtils.MusicKeysDefault().GetTitle();
        }

        #endregion

        #region Duration

        /// <summary>
        /// This field shows the duration of the song of <see cref="_setlistPosition"/>
        /// </summary>
        public string SongDuration
        {
            get
            {
                var duration= _setlistPosition.Song?.Duration ?? 0;
                return _timeFormatConverter.FormatToTimeString(duration);
            }
        }

        #endregion

        #region Genre
        /// <summary>
        /// This field shows the genre of the song of <see cref="_setlistPosition"/>
        /// </summary>
        public string SongGenre => _setlistPosition.Song?.Genre ?? string.Empty;

        #endregion

        #region Position
        /// <summary>
        /// This field provides readonly access to the songs position
        /// </summary>
        /// <remarks>
        /// IMPORTANT: This position value should ONLY be used for the UI, due to
        /// the fact that this value is one higher than the actual SetlistPosition-Position-Value
        /// in the Persistence Layer. For getting the real Position, use <see cref="GetDataPositionValue"/>
        /// </remarks>
        public int Position => _setlistPosition.Position + 1;
        #endregion

        #region MyRegion
        /// <summary>
        /// This property provides the Image-Uri for the IconedButton that should display whether the song is playing or not
        /// </summary>
        public Uri PlayIcon => (_audioPlayer.IsPlaying)
            ? new Uri($"{Application.Current.GetResource<string>(AppResourceKeys.AppBaseUri)}/resources/images/ICStop.svg")
            : new Uri($"{Application.Current.GetResource<string>(AppResourceKeys.AppBaseUri)}/resources/images/ICPlay.svg");
        

        #endregion

        #endregion

        #region Functions
        /// <summary>
        /// This function return the REAL position value of <see cref="_setlistPosition"/>
        /// like it is stored in the persistence layer
        /// </summary>
        /// <returns>the Position value of <see cref="_setlistPosition"/></returns>
        public int GetDataPositionValue() => _setlistPosition.Position;

        /// <summary>
        /// This function ensures that no audio is playing by stopping the AudioPlayer
        /// </summary>
        public void EnsureNoAudioPlaying() => _audioPlayer.Stop();
        #endregion

        #region Functions for IElementContainer
        /// <see cref="IElementContainer{T}.GetElement()"/>
        public SetlistPosition GetElement() => _setlistPosition;

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the commands
        /// </summary>
        private void ConfigCommands()
        {
            RemoveFromSetlistCommand = new DelegateCommand<object, int>(_ =>
            {
                Debug.WriteLine($"Remove Command was called for {SongTitle} at Position {Position}");
                var removeIndex = _setlistPosition.Position;
                var success = _dataService.Remove(_setlistPosition);
                return (success) ? removeIndex : -1;
            });

            AudioInteractCommand = new DelegateCommand<object, bool>(_ =>
            {
                try
                {
                    if (_audioPlayer.IsPlaying)
                    {
                        _audioPlayer.Stop();

                        if (_audioPlayer.Source.LocalPath != _setlistPosition.Song.FilePath)
                        {
                            _audioPlayer.Open(new Uri(_setlistPosition.Song.FilePath));
                            _audioPlayer.Play();
                        }
                    }
                    else
                    {
                        _audioPlayer.Stop();
                        _audioPlayer.Open(new Uri(_setlistPosition.Song.FilePath));
                        _audioPlayer.Play();
                    }
                    OnPropertyChanged(nameof(PlayIcon));
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return false;
                }

                
            }, _ => File.Exists(_setlistPosition.Song.FilePath),false);
        }

        #endregion

        
    }
}
