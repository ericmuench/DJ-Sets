using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.model.entityframework;
using DJSets.model.model_observe;
using DJSets.util.async;
using DJSets.util.Extensions;
using DJSets.util.mvvm.converters;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.song.song_item;

namespace DJSets.viewmodel.song.song_menu
{
    /// <summary>
    /// This class is the viewmodel for a SongMenuView and provides all necessary data
    /// </summary>
    public class SongMenuViewModel : BaseViewModel
    {
        #region Constructors
        public SongMenuViewModel()
        {
            SongView.CurrentChanged += OnSongViewCurrentChanged;
            ModelNotificationCenter.SongNotifier.RegisterObserver(LoadData);
        }
        #endregion

        #region Finializers
        ~SongMenuViewModel()
        {
            ModelNotificationCenter.SongNotifier.UnRegisterObserver(LoadData);
        }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk is a DataService that executes all Data operations for Song-CRUD-Operations
        /// </summary>
        private readonly IDataService<Song> _dataService = new EfSqliteSongDataService();
        #endregion

        #region SongVMs
        /// <summary>
        /// This field stores the SongVMs value
        /// </summary>
        private ObservableCollection<OverviewSongItemViewModel> _songVMs = new ObservableCollection<OverviewSongItemViewModel>();
        ///<summary>
        /// This property provides the SongVMs-Value stored in <see cref="_songVMs"/> for eventual Binding in xaml.
        /// </summary>
        private ObservableCollection<OverviewSongItemViewModel> SongVMs
        {
            get => _songVMs;
            set
            {
                Set(ref _songVMs, value, nameof(SongVMs));
                AssignDataToSongView(SongVMs);
            }
        }

        #endregion

        #region SongView
        /// <summary>
        /// This field provides stores the backing field value of <see cref="SongView"/>
        /// </summary>
        private ListCollectionView _songView = new ListCollectionView(new List<OverviewSongItemViewModel>());
        /// <summary>
        /// This field provides a <see cref="ListCollectionView"/> for the UI and further registers current-changed-Events
        /// </summary>
        public ListCollectionView SongView
        {
            get => _songView;
            private set
            {
                Set(ref _songView, value, nameof(SongView));
            }
        }

        #endregion

        #region SelectedSongVM
        /// <summary>
        /// This computed property provides the value for the currently selected element
        /// </summary>
        public ObservableCollection<OverviewSongItemViewModel> SelectedSongVms = new ObservableCollection<OverviewSongItemViewModel>();
        #endregion

        #region FilterText
        /// <summary>
        /// This field stores the FilterText value
        /// </summary>
        private string _filterText = string.Empty;
        ///<summary>
        /// This property provides the NAME-Value stored in <see cref="_filterText"/> for eventual Binding in xaml.
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(ref _filterText, value, nameof(FilterText));
                FilterData();
            }
        }
        #endregion

        #endregion
        #region Functions
        /// <summary>
        /// This function loads the viewmodel-data asynchronously and assigns it to the corresponding Fields
        /// </summary>
        public void LoadData()
        {
            new AsyncTask<List<Song>>()
                .OnExecute(() => _dataService.GetAll())
                .OnDone(allSongs =>
                {
                    allSongs.NotNull(it =>
                    {
                        SongVMs = new ObservableCollection<OverviewSongItemViewModel>(
                            it.Select(element => new OverviewSongItemViewModel(element))
                        );
                    });
                    OnLoadingFinished.Invoke();
                })
                .Start();
        }
        #endregion

        #region Help Functions
        /// <summary>
        /// This function filters data in <see cref="SongView"/> according to the Filtertext entered by the user
        /// </summary>
        /// <remarks>This function was inspired by Peter Rill, therefore LOC do not count for this functions</remarks>
        private void FilterData()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                SongView.Filter = o => true;
            }
            else
            {
                SongView.IsLiveFiltering = true;
                SongView.Filter = o =>
                {
                    if (o is OverviewSongItemViewModel vm)
                    {
                        return vm.Title.ToLower().Contains(FilterText.ToLower()) ||
                               vm.Artist.ToLower().Contains(FilterText.ToLower());
                    }

                    return true;
                };
            }
        }

        /// <summary>
        /// This function applies all data stored in <see cref="songs"/>
        /// to <see cref="SongView"/>. Further it configures the latter for grouping
        /// and event changes.
        /// </summary>
        /// <param name="songs">All songs that need to be applied to <see cref="SongView"/></param>
        /// <remarks>Partical Copied LOC: Everything after the reflection part</remarks>
        private void AssignDataToSongView(ObservableCollection<OverviewSongItemViewModel> songs)
        {
            //Getting the property name of the songs title via reflection
            var propName = typeof(OverviewSongItemViewModel)
                .GetProperties()
                .Select(property => property.Name)
                .ToList()
                .TakeIf(props => props.Count > 0, 
                    new List<string>(new[] { "Artist" }))[1];

            SongView = new ListCollectionView(songs)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propName, ListSortDirection.Ascending)}
            };

            
            SongView.GroupDescriptions?.Add(new PropertyGroupDescription(
                propName,
                new NameToInitialConverter()
            ));
            SongView.CurrentChanged += OnSongViewCurrentChanged;
        }

        #endregion

        #region Events
        /// <summary>
        /// This Action is called when <see cref="SelectedSongVms"/> has changed to react to this event
        /// from other components
        /// </summary>
        public Action<ObservableCollection<OverviewSongItemViewModel>> OnSelectedSongVmsChanged = _ => { };

        /// <summary>
        /// This function defines a event-handling routine for the CurrentChanged Event of <see cref="SongView"/>
        /// </summary>
        /// <remarks>
        /// This function also calls <see cref="OnSelectedSongVmsChanged"/> to let
        /// other components react to Changes of <see cref="SelectedSongVms"/>
        /// </remarks>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Args</param>
        private void OnSongViewCurrentChanged(object sender, EventArgs args)
        {
            OnPropertyChanged(nameof(SelectedSongVms));
            OnSelectedSongVmsChanged(SelectedSongVms);
        }

        /// <summary>
        /// This Action is called everytime when new data have been sucessfully loaded. It can be
        /// modified by the UI to add some further UI Logic
        /// </summary>
        public Action OnLoadingFinished = () => { };

        #endregion
    }
}
