using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.model.entityframework;
using DJSets.model.model_observe;
using DJSets.util.async;
using DJSets.util.Extensions;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.setlist.setlist_item;

namespace DJSets.viewmodel.setlist.setlist_menu
{
    /// <summary>
    /// This class is the viewmodel for a SetlistMenuView and provides all necessary data
    /// </summary>
    public class SetlistMenuViewModel:  BaseViewModel
    {
        #region Constructors
        public SetlistMenuViewModel()
        {
            ModelNotificationCenter.SetlistNotifier.RegisterObserver(LoadData);
        }
        #endregion

        #region Finalizers
        ~SetlistMenuViewModel()
        {
            ModelNotificationCenter.SetlistNotifier.UnRegisterObserver(LoadData);
        }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk is a DataService that executes all Data operations for Setlist-CRUD-Operations
        /// </summary>
        private readonly IDataService<Setlist> _dataService = new EfSqliteSetlistDataService();
        #endregion

        #region SetlistVMs
        /// <summary>
        /// This field stores the SetlistVMs value
        /// </summary>
        private ObservableCollection<OverviewSetlistItemViewModel> _setlistVMs = new ObservableCollection<OverviewSetlistItemViewModel>();
        ///<summary>
        /// This property provides the SetlistVMs-Value stored in <see cref="_setlistVMs"/> for eventual Binding in xaml.
        /// </summary>
        private ObservableCollection<OverviewSetlistItemViewModel> SetlistVMs
        {
            get => _setlistVMs;
            set
            {
                Set(ref _setlistVMs, value, nameof(SetlistVMs));
                AssignDataToSetlistView(SetlistVMs);
            }
        }

        #endregion

        #region SetlistView
        /// <summary>
        /// This field provides stores the backing field value of <see cref="SetlistView"/>
        /// </summary>
        private ListCollectionView _setlistView = new ListCollectionView(new List<OverviewSetlistItemViewModel>());
        /// <summary>
        /// This field provides a <see cref="ListCollectionView"/> for the UI and further registers current-changed-Events
        /// </summary>
        public ListCollectionView SetlistView
        {
            get => _setlistView;
            private set => Set(ref _setlistView, value, nameof(SetlistView));
        }

        #endregion

        #region SelectedSetlistVM
        /// <summary>
        /// This computed property provides the value for the currently selected element
        /// </summary>
        public OverviewSetlistItemViewModel SelectedSetlistVm => SetlistView.CurrentItem as OverviewSetlistItemViewModel;
        #endregion
        #endregion

        #region Functions
        /// <summary>
        /// This function loads the viewmodel-data asynchronously and assigns it to the corresponding Fields
        /// </summary>
        public void LoadData()
        {
            new AsyncTask<List<Setlist>>()
                .OnExecute(() =>
                {
                    return _dataService
                        .GetAll()
                        .OrderBy(setlist => setlist.Title)
                        .ToList();
                })
                .OnDone(setlists =>
                {
                    setlists.NotNull(it =>
                    {
                        SetlistVMs = new ObservableCollection<OverviewSetlistItemViewModel>(
                            it.Select(element => new OverviewSetlistItemViewModel(element))
                        );
                    });
                    OnLoadingFinished.Invoke();
                })
                .Start();
        }
        #endregion

        #region Help functions
        /// <summary>
        /// This function applies all data stored in <see cref="setlists"/>
        /// to <see cref="SetlistView"/>. Further it configures the latter for grouping
        /// and event changes.
        /// </summary>
        /// <param name="setlists">Setlists that need to be applied to <see cref="SetlistView"/></param>
        /// <remarks>Copied LOC: 2 (From Peter Rill MVM Selfmade)</remarks>
        private void AssignDataToSetlistView(ObservableCollection<OverviewSetlistItemViewModel> setlists)
        {
            SetlistView = new ListCollectionView(setlists);
            SetlistView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedSetlistVm));
        }
        #endregion

        #region Events
        /// <summary>
        /// This Action is called everytime when new data have been sucessfully loaded. It can be
        /// modified by the UI to add some further UI Logic
        /// </summary>
        public Action OnLoadingFinished = () => { };
        #endregion
    }
}
