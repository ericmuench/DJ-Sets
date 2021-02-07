
using System.Diagnostics;
using System.Windows;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.model.entityframework;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;
using DJSets.viewmodel.setlist.setlist_setup;
using DJSets.viewmodel.vm_util.titledescription;

namespace DJSets.viewmodel.setlist.setlist_edit
{
    public class SetlistEditViewModel : BaseViewModel, ITitleDescriptionInteractable
    {
        #region Constructors
        public SetlistEditViewModel(IElementContainer<Setlist> setlistContainer)
        {
            _setlist = setlistContainer.GetElement();
            ApplyDataToTitleDescriptionSource();
            ConfigCommands();
            
        }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk provides all necessary DB-Operations for Setlists
        /// </summary>
        private readonly IDataService<Setlist> _dataService = new EfSqliteSetlistDataService(); 
        #endregion

        #region Setlist
        /// <summary>
        /// This field is the Setlist that should be modified
        /// </summary>
        private Setlist _setlist;
        #endregion

        #region ActionTitle
        /// <summary>
        /// This field provides the ActionTitle value
        /// </summary>
        public string ActionTitle => Application.Current.GetResource<string>(StringResourceKeys.StrEditSet);
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

        #endregion

        #region Commands
        /// <summary>
        /// This Command provides functionality for saving the current Song
        /// </summary>
        public DelegateCommand<object,bool> SaveCommand { get; set; }
        /// <summary>
        /// This Command provides functionality for canceling the edit/add action for songs
        /// </summary>
        public DelegateCommand<object,bool> CancelCommand { get; set; }
        #endregion

        #region Interface Functions for TitleDescriptionInteractable
        /// <summary>
        /// Oldschool-JavaStyle-Setter for <see cref="_titleDescriptionViewModel"/>
        /// </summary>
        /// <param name="vm">new ViewModel-Istance to be set to <see cref="_titleDescriptionViewModel"/></param>
        /// <see cref="ITitleDescriptionInteractable"/>
        public void SetTitleDescriptionSource(TitleDescriptionViewModel vm)
        {
            _titleDescriptionViewModel = vm;
            _titleDescriptionViewModel.OnTitleChanged = _ => SaveCommand.NotifyCanExecuteChanged();
            ApplyDataToTitleDescriptionSource();
        }
        #endregion

        #region Help Functions

        /// <summary>
        /// This function applies Data of <see cref="_setlist"/> to <see cref="_titleDescriptionViewModel"/>
        /// </summary>
        private void ApplyDataToTitleDescriptionSource() => _titleDescriptionViewModel.NotNull(titleDescrVm =>
        {
            titleDescrVm.Title = _setlist?.Title ?? string.Empty;
            titleDescrVm.Description = _setlist?.Description ?? string.Empty;
        });
        
        /// <summary>
        /// This function configures the Commands stored in <see cref="viewmodel"/> by adding some UI-Callbacks to them
        /// </summary>
        private void ConfigCommands()
        {
            SaveCommand = new DelegateCommand<object,bool>((it) =>
            {
                Debug.WriteLine("save command called...");

                _setlist.Description = Description;
                _setlist.Title = Title;

                return _dataService.Update(_setlist);
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
