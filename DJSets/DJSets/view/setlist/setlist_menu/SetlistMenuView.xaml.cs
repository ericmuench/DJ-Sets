using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using DJSets.model.entityframework;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.view.custom_views;
using DJSets.view.setlist.setlist_autogenerate;
using DJSets.view.setlist.setlist_detail;
using DJSets.view.setlist.setlist_setup;
using DJSets.viewmodel.setlist.setlist_autogenerate;
using DJSets.viewmodel.setlist.setlist_detail;
using DJSets.viewmodel.setlist.setlist_menu;
using DJSets.viewmodel.setlist.setlist_setup;

namespace DJSets.view.setlist.setlist_menu
{
    /// <summary>
    /// This class defines the code-behind logic for the SetlistMenuView
    /// </summary>
    public partial class SetlistMenuView : Page
    {
        #region Constructors
        public SetlistMenuView(SetlistMenuViewModel vm, Page parent = null)
        {
            InitializeComponent();
            _parent = parent;
            _viewModel = vm;
            DataContext = _viewModel;
            _viewModel.OnLoadingFinished = SetlistList.UnselectAll;
            _viewModel.LoadData();
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the viewmodel for the SetlistMenuView
        /// </summary>
        private readonly SetlistMenuViewModel _viewModel;

        /// <summary>
        /// This field defines the parent of this Page due to the fact that it is programatically added to the view tree
        /// </summary>
        private Page _parent;
        #endregion

        #region Functions

        /// <summary>
        /// This function navigates to a <see cref="SetlistDetailView"/> for a certain Setlist
        /// </summary>
        /// <param name="elementContainer">Container that is able to emit a Setlist</param>
        private void OpenSetlistDetails(IElementContainer<Setlist> elementContainer)
        {
            NavigationService
                .GetNavigationService(_parent)?
                .Navigate(new SetlistDetailView(new SetlistDetailViewModel(elementContainer)));
        }

        /// <summary>
        /// This function is called when user clicks Button to create new Setlist
        /// </summary>
        /// <param name="sender">Sender of the click event which is the Setlist-Add-Button</param>
        /// <param name="e">the click-event-EventArgs</param>
        private void OnCreateNewSetlist(object sender, RoutedEventArgs e)
        {
            string title = this.GetResource<string>(StringResourceKeys.StrCreateNewSet);
            var setupVm = new SetlistSetupViewModel();
            var dialogResult = new CustomDialogWindow(new SetlistSetupView(setupVm),title).ShowDialog();

            //result
            dialogResult.NotNull(setlistCreated =>
            {
                if (setlistCreated)
                {
                    OpenSetlistDetails(setupVm);
                }
            });
        }


        /// <summary>
        /// This function removes all selected items of <see cref="SetlistList"/> when right clicking an element.
        /// </summary>
        /// <param name="sender">sender of the event, usually an item of <see cref="SetlistList"/></param>
        /// <param name="e">Event Args</param>
        private void OnSetlistItemRightClicked(object sender, MouseButtonEventArgs e) => SetlistList.UnselectAll();


        /// <summary>
        /// This function reacts when user left-clicks the background of this page
        /// </summary>
        /// <param name="sender">Sender, usually the Background, meaning the whole Layout</param>
        /// <param name="e">Event Args</param>
        private void OnBackgroundLeftClicked(object sender, MouseButtonEventArgs e) => SetlistList.UnselectAll();
        #endregion

        /// <summary>
        /// This function handles clicking an element of <see cref="SetlistList"/>
        /// </summary>
        /// <param name="sender">sender of the event, usually an item of <see cref="SetlistList"/></param>
        /// <param name="e">Event Args</param>
        private void OnSetlistItemLeftClicked(object sender, MouseButtonEventArgs e)
        {
            _viewModel.SelectedSetlistVm.NotNull(OpenSetlistDetails);
        }

        private void OnAutoGenerateSetlist(object sender, RoutedEventArgs e)
        {
            var title = Application.Current.GetResource<string>(StringResourceKeys.StrAutogenerateSet);
            new CustomDialogWindow(
                new SetlistAutogenerateView(new SetlistAutogenerateViewModel()),
                title, 
                window =>
                {
                    window.WindowState = WindowState.Maximized;
                })
            .ShowDialog();
        }
    }
}
