using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.clerks.timeformat;
using DJSets.model.entityframework;
using DJSets.model.musickeys;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.util.mvvm.validation;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.song.song_detail
{
    /// <summary>
    /// This class is the viewmodel for a SongDetailView and provides all necessary data
    /// </summary>
    public class SongDetailViewModel : ValidatableViewModel
    {
        #region Constructors
        public SongDetailViewModel(IElementContainer<Song> songContainer = null) : this(songContainer?.GetElement()) { }

        public SongDetailViewModel(Song song)
        {
            //configure Commands
            ConfigCommands();

            //assignSong Data
            _song = song;
            AssignSongDataToViewModel();
        }
        #endregion

        #region Commands
        /// <summary>
        /// This Command provides functionality for saving the current Song
        /// </summary>
        public DelegateCommand<object,bool> SaveCommand { get; set; }
        /// <summary>
        /// This Command provides functionality for deleting the current Song
        /// </summary>
        public DelegateCommand<object, bool> DeleteCommand { get; set; }
        /// <summary>
        /// This Command provides functionality for canceling the edit/add action for songs
        /// </summary>
        public DelegateCommand<object, bool> CancelCommand { get; set; }
        #endregion

        #region Fields

        #region Helper Clerks
        /// <summary>
        /// This clerk helps converting timestamps into time-values displayable in the DurationString-Textfield in
        /// the corresponding view for this viewmodel 
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();
        
        /// <summary>
        /// This clerk helps validating certain fields in this ViewModel to seperate validation logic from viewmodel code
        /// </summary>
        private readonly SongDetailViewModelValidator _validator = new SongDetailViewModelValidator();
        
        /// <summary>
        /// This clerk is a DataService that executes all Data operations for Song-CRUD-Operations
        /// </summary>
        private readonly IDataService<Song> _dataService = new EfSqliteSongDataService();
        #endregion

        #region Song
        /// <summary>
        /// This field stores the song to be displayed and edited
        /// </summary>
        private Song _song;
        #endregion

        #region Action Title
        /// <summary>
        /// This field returns the headline-title for the view depending on the fact whether a somg is new or should be added
        /// </summary>
        public string ActionTitle
        {
            get
            {
                if (_song == null)
                {
                    return Application.Current.GetResource<string>(StringResourceKeys.StrAddSong);
                }

                return Application.Current.GetResource<string>(StringResourceKeys.StrEditSong);

            }
        }
        #endregion

        #region Title

        /// <summary>
        /// This field stores the title of the song
        /// </summary>
        private string _title = string.Empty;
        ///<summary>
        /// This property provides the Title-Value stored in <see cref="_title"/> for Binding in xaml.
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set((val,propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrFileNameCannotBeEmptyOrBlank);
                return SetError(() => _validator.IsValidTitle(val),propName,errMsg);
            },ref _title, value, nameof(Title), s => NotifyValueChangeToCommands());
        }
        #endregion

        #region Artist
        /// <summary>
        /// This field stores the artist of the song
        /// </summary>
        private string _artist = string.Empty;
        ///<summary>
        /// This property provides the Title-Value stored in <see cref="_artist"/> for Binding in xaml.
        /// </summary>
        public string Artist
        {
            get => _artist;
            set => Set((val, propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrSongArtistCannotBeEmptyOrBlank);
                return SetError(() => _validator.IsValidArtist(val), propName, errMsg);
            },ref _artist, value, nameof(Artist), s => NotifyValueChangeToCommands());
        }
        #endregion

        #region FilePath
        /// <summary>
        /// This field stores the current selected File path
        /// </summary>
        private string _filePath = string.Empty;
        ///<summary>
        /// This property provides the currently selected FilePath-Value stored in <see cref="_filePath"/> for Binding in xaml.
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set => Set((val, propName) =>
            {
                var errMsg =
                    Application.Current.GetResource<string>(StringResourceKeys
                        .StrFilePathCanOnlyBeEmptyOrExistingFile);
                return SetError(() => _validator.IsValidFilePath(val), propName, errMsg);
            }, ref _filePath, value, nameof(FilePath), _ => NotifyValueChangeToCommands());
        }
        #endregion

        #region MusicKey

        /// <summary>
        /// This field defines all possible items for the music keys selection
        /// </summary>
        public IEnumerable<string> AllMusicKeyTitles => MusicKeysUtils.AllValues().Select(it => it.GetTitle());

        /// <summary>
        /// This field stores the current selected MusicKeys-Value
        /// </summary>
        private MusicKeys _selectedMusicKey = MusicKeysUtils.MusicKeysDefault();
        ///<summary>
        /// This property provides the currently selected MusicKeys-Value stored in <see cref="_selectedMusicKey"/> for Binding in xaml.
        /// </summary>
        public MusicKeys SelectedMusicKey
        {
            get => _selectedMusicKey;
            set => Set(ref _selectedMusicKey, value, nameof(SelectedMusicKey), _ => NotifyValueChangeToCommands());
        }

        #endregion

        #region Tempo
        /// <summary>
        /// This field stores the current selected Tempo value in BPM
        /// </summary>
        private string _tempo = "0";
        ///<summary>
        /// This property provides the currently selected Tempo-Value stored in <see cref="_tempo"/> for Binding in xaml.
        /// </summary>
        public string Tempo
        {
            get => _tempo;
            set => Set((val, propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrBpmValueInvalid);
                return SetError(() => _validator.IsValidTempo(val), propName, errMsg);
            },ref _tempo, value, nameof(Tempo), s => NotifyValueChangeToCommands());
        }
        #endregion

        #region Genre
        /// <summary>
        /// This field stores the Genre of the song
        /// </summary>
        private string _genre = string.Empty;
        ///<summary>
        /// This property provides the currently selected Genre-Value stored in <see cref="_genre"/> for Binding in xaml.
        /// </summary>
        public string Genre
        {
            get => _genre;
            set => Set(ref _genre, value, nameof(Genre), _ => NotifyValueChangeToCommands());
        }
        #endregion

        #region DurationString
        /// <summary>
        /// This field stores the duration of the song
        /// </summary>
        private string _duration = "00:00";
        ///<summary>
        /// This property provides the DurationString-Value stored in <see cref="_duration"/> for Binding in xaml.
        /// </summary>
        public string Duration
        {
            get => _duration;
            set => Set((val, propName) =>
            {
                Debug.WriteLine($"Validating a new duration value: {val}");
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrDurationInvalid);
                var valid = _validator.IsValidDuration(val);
                return SetError(() => valid, propName, errMsg);
            },ref _duration, value, nameof(Duration), s => NotifyValueChangeToCommands());
        }
        #endregion
        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the Commands stored in <see cref="viewmodel"/> by adding some UI-Callbacks to them
        /// </summary>
        private void ConfigCommands()
        {
            SaveCommand = new DelegateCommand<object,bool>((it) =>
            {
                if (_song == null)
                {
                    //Song does not exist --> perform add
                    AssignViewModelDataToSong();
                    var success = _dataService.Add(_song);
                    if (!success)
                    {
                        //This assignment is necessary to avoid bugs for a retry after a failed operation
                        _song = null;
                    }
                    Debug.WriteLine($"Update operation was successful : {success}");
                    return success;
                }
                else
                {
                    //song does exist --> perform update
                    AssignViewModelDataToSong();
                    var success = _dataService.Update(_song);
                    Debug.WriteLine($"Update operation was successful : {success}");
                    return success;
                }

            }, _ => _validator.IsValid(this));
            
            DeleteCommand = new DelegateCommand<object,bool>((it) =>
            {
                return _dataService.Remove(_song);
            }, _ => _song != null 
                    && _song.Title == Title
                    && _song.Artist == Artist
                    && _song.FilePath == FilePath
                    && _song.MusicKey == SelectedMusicKey
                    && _song.Genre == Genre
                    && _song.Bpm.ToString() == Tempo
                    && _timeFormatConverter.FormatToTimeString(_song.Duration) == Duration
            );

            CancelCommand = new DelegateCommand<object,bool>((it) => true);
        }

        /// <summary>
        /// This Function informs all command that some data has changed and that they should check whether
        /// they can be executed or not.
        /// </summary>
        private void NotifyValueChangeToCommands()
        {
            SaveCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// This function assigns data stored in <see cref="_song"/> into the ViewModels Fields for Binding
        /// </summary>
        private void AssignSongDataToViewModel() => _song.NotNull(songNn =>
        {
            Title = songNn.Title;
            Artist = songNn.Artist;
            FilePath = songNn.FilePath;
            SelectedMusicKey = songNn.MusicKey;
            Tempo = songNn.Bpm.ToString();
            Genre = songNn.Genre;
            Duration = _timeFormatConverter.FormatToTimeString(songNn.Duration);
        });

        /// <summary>
        /// This function assigns Data from VM to the <see cref="_song"/>-Object. If the <see cref="_song"/>-Object
        /// is null, a new object will be created.
        /// </summary>
        private void AssignViewModelDataToSong()
        {
            _song ??= new Song();

            _song.Title = Title;
            _song.Artist = Artist;
            _song.FilePath = FilePath;
            _song.MusicKey = SelectedMusicKey;
            _song.Bpm = int.Parse(Tempo);
            _song.Duration = _timeFormatConverter.FormatToTimeMillis(Duration);
            _song.Genre = Genre;
        }

        #endregion
    }
}
