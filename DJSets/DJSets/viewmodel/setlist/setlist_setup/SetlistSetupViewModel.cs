using System.Diagnostics;
using System.Windows;
using DJSets.clerks.dataservices.entityframework;
using DJSets.model.entityframework;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.vm_util.titledescription;

namespace DJSets.viewmodel.setlist.setlist_setup
{
    /// <summary>
    /// This class is the viewmodel for a SetlistSetupView and provides all necessary data
    /// </summary>
    public class SetlistSetupViewModel : BaseViewModel, ITitleDescriptionInteractable, IElementContainer<Setlist>
    {
        #region Constructors
        public SetlistSetupViewModel()
        {
            //configure Commands
            ConfigCommands();
        }
        #endregion



        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk provides all Interaction logic with the database for setlists
        /// </summary>
        private readonly EfSqliteSetlistDataService _dataService = new EfSqliteSetlistDataService();

        #endregion

        #region ActionTitle

        /// <summary>
        /// This field provides the ActionTitle value
        /// </summary>
        public string ActionTitle => Application.Current.GetResource<string>(StringResourceKeys.StrCreateNewSet);

        #endregion

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

        #region Setlist
        private Setlist _setlist;

        #endregion

        #endregion



        #region Commands
        /// <summary>
        /// This Command provides functionality for saving the current Song
        /// </summary>
        public DelegateCommand<object,bool> CreateCommand { get; set; }
        /// <summary>
        /// This Command provides functionality for canceling the edit/add action for songs
        /// </summary>
        public DelegateCommand<object,bool> CancelCommand { get; set; }
        #endregion

        #region Interface Functions for ITitleDescriptionInteractable
        /// <summary>
        /// Oldschool-JavaStyle-Setter for <see cref="_titleDescriptionViewModel"/>
        /// </summary>
        /// <param name="vm">new ViewModel-Istance to be set to <see cref="_titleDescriptionViewModel"/></param>
        /// <see cref="ITitleDescriptionInteractable"/>
        public void SetTitleDescriptionSource(TitleDescriptionViewModel vm)
        {
            _titleDescriptionViewModel = vm;
            _titleDescriptionViewModel.OnTitleChanged = _ => CreateCommand.NotifyCanExecuteChanged();
        }
        #endregion

        #region Interface Functions for IElementContainer
        /// <see cref="IElementContainer{T}.GetElement()"/>
        public Setlist GetElement() => _setlist;
        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the Commands stored in <see cref="viewmodel"/> by adding some UI-Callbacks to them
        /// </summary>
        private void ConfigCommands()
        {
            CreateCommand = new DelegateCommand<object,bool>((it) =>
            {
                Debug.WriteLine("create command called...");
                var createdSetlist = new Setlist()
                {
                    Description = this.Description,
                    Title = this.Title
                };

                var addSuccess = _dataService.Add(createdSetlist);
                if (addSuccess)
                {
                    _setlist = createdSetlist;
                }
                return addSuccess;
            }, _ => !string.IsNullOrWhiteSpace(_titleDescriptionViewModel.Title));

            CancelCommand = new DelegateCommand<object,bool>((it) =>
            {
                Debug.WriteLine("cancel command called...");
                return true;
            });
        }
        #endregion
    }
}
