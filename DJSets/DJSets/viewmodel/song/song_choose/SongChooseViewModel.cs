
using System.Linq;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.model.entityframework;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.song.song_menu;

namespace DJSets.viewmodel.song.song_choose
{
    /// <summary>
    /// This class defines the ViewModel for a SongChooseView
    /// </summary>
    public class SongChooseViewModel : BaseViewModel
    {
        #region Constructors
        public SongChooseViewModel(IElementContainer<Setlist> setlistContainer)
        {
            _dataService = new EfSqliteSetlistPositionDataService(setlistContainer.GetElement());
            _setlist = setlistContainer.GetElement();
            ConfigCommands();
        }
        #endregion

        #region Commands
        /// <summary>
        /// This Command handles the cancel Action
        /// </summary>
        public DelegateCommand<object,bool> CancelCommand { get; set; }

        /// <summary>
        /// This command takes a choosen Song and adds it to the given Setlist
        /// </summary>
        public DelegateCommand<object,bool> ChooseCommand { get; set; }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk provides all necessary data operations in DB for SetlistPositions
        /// </summary>
        private readonly IExtendedModelOperationsDataService<SetlistPosition> _dataService;

        #endregion

        #region Setlist
        /// <summary>
        /// This field provides READ-Access to Setlist
        /// </summary>
        private readonly Setlist _setlist;

        #endregion
        #region SongMenuViewModel
        private SongMenuViewModel _songMenuViewModel;

        public SongMenuViewModel SongMenuViewModel
        {
            get => _songMenuViewModel;
            set
            {
                value.OnSelectedSongVmsChanged = _ =>
                {
                    OnPropertyChanged(nameof(SelectedSongText));
                    ChooseCommand.NotifyCanExecuteChanged();
                };
                _songMenuViewModel = value;
                OnPropertyChanged(nameof(SelectedSongText));
                
            }
        }
        #endregion

        #region SelectedSongText
        ///<summary>
        /// This property provides the SelectedSongText-Value stored in <see cref="SongMenuViewModel"/> for eventual Binding in xaml.
        /// </summary>
        public string SelectedSongText
        {
            get 
            {
                var selectedSongVms = SongMenuViewModel.SelectedSongVms;

                return (selectedSongVms != null && selectedSongVms.Count > 0)
                    ? string.Join(",", 
                        selectedSongVms.Select(it => $"{it.Artist} - {it.Title}"))
                    : string.Empty;
            }
        }
        #endregion

        #endregion

        #region Functions
        /// <summary>
        /// This function defines logic for informing certain components that the song-
        /// selection has changed
        /// </summary>
        public void NotifySongSelectionChanged()
        {
            ChooseCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(SelectedSongText));
        }

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures all Commands
        /// </summary>
        private void ConfigCommands()
        {
            CancelCommand = new DelegateCommand<object, bool>(_ => true);

            ChooseCommand = new DelegateCommand<object, bool>(_ =>
            {
                var basePosition = _dataService.NumberOfElements();
                return _dataService.AddAll(SongMenuViewModel
                    .SelectedSongVms
                    .Select(vm => new SetlistPosition()
                    {
                        Setlist = _setlist,
                        Song = vm.GetElement(),
                        Position = basePosition++
                    }).ToList());
            }, canExe: _ => _songMenuViewModel.SelectedSongVms != null 
                            && _songMenuViewModel.SelectedSongVms.Count > 0);
        }

        #endregion
    }
}
