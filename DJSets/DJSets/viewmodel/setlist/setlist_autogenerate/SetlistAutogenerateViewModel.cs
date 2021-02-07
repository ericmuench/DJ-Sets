using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DJSets.clerks.autogeneration;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.clerks.timeformat;
using DJSets.model.autogeneration;
using DJSets.model.entityframework;
using DJSets.model.musickeys;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.util.mvvm.validation;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.setlist.setlist_setup;
using DJSets.viewmodel.vm_util.titledescription;

namespace DJSets.viewmodel.setlist.setlist_autogenerate
{
    /// <summary>
    /// This class defines the ViwModel for a SetlistAutogenerateView
    /// </summary>
    public class SetlistAutogenerateViewModel : ValidatableViewModel, ITitleDescriptionInteractable
    {
        #region Constructors

        public SetlistAutogenerateViewModel()
        {
            ConfigCommands();
            ConfigBackgroundWorker();
        }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This field contains validation logic for this viewmodel
        /// </summary>
        private readonly SetlistAutogenerateViewModelValidator _validator = new SetlistAutogenerateViewModelValidator();

        /// <summary>
        /// This clerks with the conversion to ms from a time string
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();

        /// <summary>
        /// This clerk allows to generate SetlistPositions according to the given parameters
        /// </summary>
        private readonly SetlistPositionGenerator _setlistPositionGenerator = new SetlistPositionGenerator(new EfSqliteSongDataService());

        /// <summary>
        /// This clerk allows persistence operations for Setlists
        /// </summary>
        private readonly IDataService<Setlist> _dataService = new EfSqliteSetlistDataService();
        #endregion

        #region Commands
        /// <summary>
        /// This command defines the cancel-functionality
        /// </summary>
        public DelegateCommand<object,bool> CancelCommand { get; set; }

        /// <summary>
        /// This command defines the save-functionality
        /// </summary>
        public DelegateCommand<object, bool> SaveCommand { get; set; }

        /// <summary>
        /// This command defines the autogenerate-functionality
        /// </summary>
        public DelegateCommand<object, object> AutogenerateCommand { get; set; }


        #endregion

        #region BackgroundWorker
        /// <summary>
        /// This field can execute the autogeneration process in background and show progress to UI
        /// </summary>
        public BackgroundWorker AutoGenerationBackgroundWorker = new BackgroundWorker();

        #region StatusText
        /// <summary>
        /// This field stores the StatusText value
        /// </summary>
        private string _statusText = string.Empty;
        ///<summary>
        /// This property provides the StatusText-Value stored in <see cref="_statusText"/> for eventual Binding in xaml.
        /// </summary>
        /// <remarks>This field should display the progress of <see cref="BackgroundWorker"/> in a Textfield</remarks>
        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value, nameof(StatusText));
        }
        #endregion
        
        #endregion

        #region GenerateModes
        /// <summary>
        /// This field defines all possible items for the MovementGenerationMode-selection
        /// </summary>
        public IEnumerable<string> AllMovementGenerationModes
            => MovementGenerationModeUtils
                .AllValues()
                .Select(it => it.GetTitle());

        /// <summary>
        /// This field defines all possible items for the MovementGenerationMode-selection
        /// </summary>
        public IEnumerable<string> AllGeneralSpecificGenerationModes
            => GeneralSpecificGenerationModeUtils
                .AllValues()
                .Select(it => it.GetTitle());
        #endregion

        #region TitleDescription

        #region TitleDescriptionViewModel
        /// <summary>
        /// This field guarantees access to title and description
        /// </summary>
        private TitleDescriptionViewModel _titleDescriptionViewModel;
        #endregion

        #region Title
        /// <summary>
        /// This property gives access to title of <see cref="TitleDescriptionViewModel"/>
        /// </summary>
        public string Title => _titleDescriptionViewModel.Title;

        #endregion

        #region Description
        /// <summary>
        /// This property gives access to description of <see cref="TitleDescriptionViewModel"/>
        /// </summary>
        public string Description => _titleDescriptionViewModel.Description;
        #endregion

        #endregion

        #region Tempo Area

        #region TempoGenerationMode
        /// <summary>
        /// This field stores the TempoGenerationMode value
        /// </summary>
        private MovementGenerationMode _tempoGenerationMode = MovementGenerationModeUtils.MovementGenerationModesDefault();
        ///<summary>
        /// This property provides the TempoGenerationMode-Value stored in <see cref="_tempoGenerationMode"/> for eventual Binding in xaml.
        /// </summary>
        public MovementGenerationMode TempoGenerationMode
        {
            get => _tempoGenerationMode;
            set => Set(ref _tempoGenerationMode, value, nameof(TempoGenerationMode));
        }
        #endregion

        #region StartTempo
        /// <summary>
        /// This field stores the StartTempo value
        /// </summary>
        private string _startTempo = "0";
        ///<summary>
        /// This property provides the StartTempo-Value stored in <see cref="_startTempo"/> for eventual Binding in xaml.
        /// </summary>
        public string StartTempo
        {
            get => _startTempo;
            set => Set((val, propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrBpmValueInvalid);
                return SetError(() =>  _validator.IsValidTempo(val), propName, errMsg);
            },ref _startTempo, value, nameof(StartTempo), _ => NotifyValueChangeToCommands());
        }
        #endregion

        #region UseSongsWith0Bpm
        /// <summary>
        /// This field stores the UseSongsWith0Bpm value
        /// </summary>
        private bool _useSongsWith0Bpm;
        ///<summary>
        /// This property provides the UseSongsWith0Bpm-Value stored in <see cref="_useSongsWith0Bpm"/> for eventual Binding in xaml.
        /// </summary>
        public bool UseSongsWith0Bpm
        {
            get => _useSongsWith0Bpm;
            set => Set(ref _useSongsWith0Bpm, value, nameof(UseSongsWith0Bpm));
        }
        #endregion

        #endregion

        #region Music Key  Area

        #region MusicKeyGenerationMode
        /// <summary>
        /// This field stores the MusicKeyGenerationMode value
        /// </summary>
        private MovementGenerationMode _musicKeyGenerationMode = MovementGenerationModeUtils.MovementGenerationModesDefault();
        ///<summary>
        /// This property provides the MusicKeyGenerationMode-Value stored in <see cref="_musicKeyGenerationMode"/> for eventual Binding in xaml.
        /// </summary>
        public MovementGenerationMode MusicKeyGenerationMode
        {
            get => _musicKeyGenerationMode;
            set => Set(ref _musicKeyGenerationMode, value, nameof(MusicKeyGenerationMode));
        }
        #endregion

        #region MusicKey
        /// <summary>
        /// This field defines all possible items for the music keys selection
        /// </summary>
        public IEnumerable<string> AllMusicKeyTitles => MusicKeysUtils.AllValues().Select(it => it.GetTitle());

        /// <summary>
        /// This field stores the current selected StartMusicKeys-Value
        /// </summary>
        private MusicKeys _startMusicKey = MusicKeysUtils.MusicKeysDefault();
        ///<summary>
        /// This property provides the currently selected StartMusicKeys-Value stored in <see cref="_startMusicKey"/> for Binding in xaml.
        /// </summary>
        public MusicKeys StartMusicKey
        {
            get => _startMusicKey;
            set => Set(ref _startMusicKey, value, nameof(StartMusicKey), _ => NotifyValueChangeToCommands());
        }
        #endregion
        
        #region UseSongsWithoutKey
        /// <summary>
        /// This field stores the UseSongsWithoutKey value
        /// </summary>
        /// <remarks>
        /// This field describes whether songs with a MusicKey-Value of Unidentified should be used further to the selected key
        /// </remarks>
        private bool _useSongsWithoutKey;
        ///<summary>
        /// This property provides the UseSongsWithoutKey-Value stored in <see cref="_useSongsWithoutKey"/> for eventual Binding in xaml.
        /// </summary>
        public bool UseSongsWithoutKey
        {
            get => _useSongsWithoutKey;
            set => Set(ref _useSongsWithoutKey, value, nameof(UseSongsWithoutKey));
        }
        #endregion

        #endregion

        #region Genre Area

        #region GenreGenerationMode
        /// <summary>
        /// This field stores the GenreGenerationMode value
        /// </summary>
        private GeneralSpecificGenerationMode _genreGenerationMode 
            = GeneralSpecificGenerationModeUtils.GeneralSpecificGenerationModesDefault();
        ///<summary>
        /// This property provides the GenreGenerationMode-Value stored in <see cref="_genreGenerationMode"/>
        /// for eventual Binding in xaml.
        /// </summary>
        public GeneralSpecificGenerationMode GenreGenerationMode
        {
            get => _genreGenerationMode;
            set
            {
                Set(ref _genreGenerationMode, value, nameof(GenreGenerationMode));
                OnPropertyChanged(nameof(CanEditStartGenre));
            }
        }
        #endregion

        #region StartGenre
        /// <summary>
        /// This field stores the StartGenre value
        /// </summary>
        private string _startGenre = string.Empty;
        ///<summary>
        /// This property provides the StartGenre-Value stored in <see cref="_startGenre"/> for eventual Binding in xaml.
        /// </summary>
        public string StartGenre
        {
            get => _startGenre;
            set => Set(ref _startGenre, value, nameof(StartGenre));
        }
        #endregion

        #region CanEditStartGenre

        /// <summary>
        /// This field defines whether it is possible to Edit the <see cref="StartGenre"/>-
        /// Value based on <see cref="GenreGenerationMode"/>
        /// </summary>
        public bool CanEditStartGenre => GenreGenerationMode == GeneralSpecificGenerationMode.Specific;

        #endregion

        #endregion

        #region Length/Duration Area

        #region SetlistLength
        /// <summary>
        /// This field stores the SetlistLength value
        /// </summary>
        private string _setlistLength = "00:00";
        ///<summary>
        /// This property provides the SetlistLength-Value stored in <see cref="_setlistLength"/> for eventual Binding in xaml.
        /// </summary>
        public string SetlistLength
        {
            get => _setlistLength;
            set => Set((val, propName) =>
            {
                var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrDurationInvalid);
                return SetError(() => _validator.IsValidLength(val), propName, errMsg);
            },ref _setlistLength, value, nameof(SetlistLength), _ => NotifyValueChangeToCommands());
        }
        #endregion

        #endregion

        #region SetlistPositionsFromAutogeneration
        /// <summary>
        /// This field stores the SetlistPositionsFromAutogeneration value, which contains all SetlistPosition that are found after the
        /// Autogeneration was completed. The value of null indicates that Autogeneration has not been started yet.
        /// </summary>
        private List<SetlistPosition> _setlistPositionsFromAutogeneration;
        ///<summary>
        /// This property provides the SetlistPositionsFromAutogeneration-Value stored in <see cref="_setlistPositionsFromAutogeneration"/> for eventual Binding in xaml.
        /// </summary>
        public List<SetlistPosition> SetlistPositionsFromAutogeneration
        {
            get => _setlistPositionsFromAutogeneration;
            set => Set(ref _setlistPositionsFromAutogeneration, value, nameof(SetlistPositionsFromAutogeneration));
        }
        #endregion


        #endregion

        #region Functions
        /// <summary>
        /// This function cheks if the overall length of all setlistpositions in <see cref="SetlistPositionsFromAutogeneration"/>
        /// is shorter than the value in <see cref="SetlistLength"/>
        /// </summary>
        /// <returns>Whether the overall length of all setlistpositions in <see cref="SetlistPositionsFromAutogeneration"/>
        /// is shorter than the value in <see cref="SetlistLength"/> </returns>
        public bool SetlistWillBeShorterThanExpected()
        {
            var expectedLength = _timeFormatConverter.FormatToTimeMillis(SetlistLength);
            var actualLength = SetlistPositionsFromAutogeneration.Select(pos => pos.Song.Duration).Sum();
            
            return actualLength < expectedLength;
        }



        #endregion

        #region Interface Functions for ITitleDescriptionInteractable
        /// <see cref="ITitleDescriptionInteractable.SetTitleDescriptionSource"/>
        public void SetTitleDescriptionSource(TitleDescriptionViewModel vm) => vm.NotNull(it =>
        {
            _titleDescriptionViewModel = it;
            _titleDescriptionViewModel.OnTitleChanged = _ => NotifyValueChangeToCommands();
        });


        #endregion

        #region Overriden Components
        /// <summary>
        /// Override HasErrors by from class
        /// </summary>
        public override bool HasErrors => base.HasErrors || _titleDescriptionViewModel.HasErrors;

        #endregion

        #region Help functions
        /// <summary>
        /// This function configures all commands
        /// </summary>
        private void ConfigCommands()
        {
            CancelCommand = new DelegateCommand<object, bool>(_ =>
            {
                if (!AutoGenerationBackgroundWorker.IsBusy)
                {
                    return true;
                }

                try
                {
                    AutoGenerationBackgroundWorker.CancelAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
            }, _ => !AutoGenerationBackgroundWorker.CancellationPending);

            SaveCommand = new DelegateCommand<object, bool>(_ =>
            {
                var setlist = new Setlist()
                {
                    Title = this.Title,
                    Description = this.Description,
                    SetlistPositions = SetlistPositionsFromAutogeneration
                };
                return _dataService.Add(setlist);
            }, _ => _validator.IsValid(this) && SetlistPositionsFromAutogeneration != null);

            AutogenerateCommand = new DelegateCommand<object, object>(_ =>
            {
                AutoGenerationBackgroundWorker.RunWorkerAsync();
                NotifyValueChangeToCommands();
                return null;
            }, _ => _validator.IsValidAutogenerationParameters(this) 
                    && !AutoGenerationBackgroundWorker.IsBusy,false);
        }

        #region BackgroundWorker

        /// <summary>
        /// This function configures <see cref="AutoGenerationBackgroundWorker"/>
        /// </summary>
        private void ConfigBackgroundWorker()
        {
            AutoGenerationBackgroundWorker.WorkerSupportsCancellation = true;
            AutoGenerationBackgroundWorker.WorkerReportsProgress = true;
            AutoGenerationBackgroundWorker.RunWorkerCompleted += OnSongAutogenerationCompleted;
            AutoGenerationBackgroundWorker.DoWork += OnFindSongsForSetlistAutogeneration;
            AutoGenerationBackgroundWorker.ProgressChanged += (sender, args) =>
            {
                StatusText = $"{args.ProgressPercentage}%";
            };
        }

        /// <summary>
        /// This function is executed by <see cref="AutoGenerationBackgroundWorker"/> to find songs for the Setlist
        /// that should be generated.
        /// </summary>
        /// <param name="sender">The sender, usually <see cref="AutoGenerationBackgroundWorker"/></param>
        /// <param name="e">Event Args</param>
        private void OnFindSongsForSetlistAutogeneration(object sender, DoWorkEventArgs e)
        {
            var args = new SetlistAutogeneration(
                TempoGenerationMode,
                int.Parse(StartTempo),
                UseSongsWith0Bpm,
                MusicKeyGenerationMode,
                StartMusicKey,
                UseSongsWithoutKey,
                GenreGenerationMode,
                StartGenre,
                _timeFormatConverter.FormatToTimeMillis(SetlistLength));

            e.Result = _setlistPositionGenerator.GenerateSetlistPositions(AutoGenerationBackgroundWorker, args);
            
        }

        /// <summary>
        /// This function is called when <see cref="AutoGenerationBackgroundWorker"/> has completed its work, meaning that
        /// he has found some songs for Autogeneration.
        /// </summary>
        /// <param name="sender">The sender, usually <see cref="AutoGenerationBackgroundWorker"/></param>
        /// <param name="e">Event Args</param>
        private void OnSongAutogenerationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetlistPositionsFromAutogeneration = e.Result as List<SetlistPosition>;

            //status
            SetlistPositionsFromAutogeneration.NotNull(positions =>
            {
                var app = Application.Current;
                var setlistLength = positions.Select(it => it.Song.Duration).Sum();
                string statusText;
                if (setlistLength == 0)
                {
                    statusText = app.GetResource<string>(StringResourceKeys.StrNoAutogenerationSetlistPositionsFound);
                }
                else
                {
                    var statusTemplate = app.GetResource<string>(StringResourceKeys.StrTemplateAutogeneratedSetlistPositions);
                    var lengthTemplate = app.GetResource<string>(StringResourceKeys.StrTemplateSetlistDuration);

                    var positonsStatus = positions
                        .Select(pos => $"{pos.Position + 1})\t {pos.Song.Artist} - {pos.Song.Title} ({pos.Song.MusicKey.GetTitle()}, {pos.Song.Bpm} BPM)");

                    var lengthStatus = lengthTemplate.Replace("#", _timeFormatConverter.FormatToTimeString(setlistLength));
                    statusText = statusTemplate.Replace("#", $"\n\n{string.Join("\n", positonsStatus)}\n\n{lengthStatus}");
                }

                StatusText = statusText;
            });

            NotifyValueChangeToCommands();
        }

        #endregion


        /// <summary>
        /// This Function informs all command that some data has changed and that they should check whether
        /// they can be executed or not.
        /// </summary>
        private void NotifyValueChangeToCommands()
        {
            SaveCommand.NotifyCanExecuteChanged();
            AutogenerateCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }
        #endregion


    }
}
