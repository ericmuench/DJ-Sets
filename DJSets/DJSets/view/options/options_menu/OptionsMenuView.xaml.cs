using System.Windows;
using System.Windows.Controls;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.view.custom_views;
using DJSets.view.filescan;
using DJSets.viewmodel.filescan;
using DJSets.viewmodel.options;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DJSets.view.options.options_menu
{
    /// <summary>
    /// This Class defines the code-behind logic for the OptionsView of the MainMenu
    /// </summary>
    public partial class OptionsMenuView : Page
    {
        #region Constructors
        public OptionsMenuView(OptionsViewModel vm)
        {
            InitializeComponent();
            _viewModel = vm;
            DataContext = _viewModel;

            ConfigViewModelCommands();
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the viewmodel for the OptionsMenuView
        /// </summary>
        private readonly OptionsViewModel _viewModel;
        #endregion

        #region Functions
        /// <summary>
        /// This function is triggered when the File-Button is pressed
        /// </summary>
        /// <param name="sender">the sender of the click event which is the File-Button</param>
        /// <param name="e">Button-Click-Eventargs</param>
        private void OnChooseScanPath(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog().Apply(it =>
            {
                it.IsFolderPicker = true;
                it.Multiselect = false;
            });

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                _viewModel.ScanPath = dialog.FileName;
            }
        }
        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the viewmodel commands by applying extra ui callbacks to them
        /// </summary>
        private void ConfigViewModelCommands()
        {
            _viewModel.SaveMusicScanPathCommand.OnDone(success =>
            {
                var msgKey = success
                    ? StringResourceKeys.StrSettingsSaveOperationSuccess
                    : StringResourceKeys.StrSettingsSaveOperationFailed;
                var img = success ? MessageBoxImage.Information : MessageBoxImage.Error;
                this.ShowMessageBoxWithResources(
                    StringResourceKeys.StrNote,
                    msgKey,
                    MessageBoxButton.OK,
                    img
                );
            });
        }

        #endregion

        /// <summary>
        /// This function handles the start of a filescan
        /// </summary>
        /// <param name="sender">sender of the action</param>
        /// <param name="e">Event Args</param>
        private void OnStartMusicFileScan(object sender, RoutedEventArgs e)
        {
            var title = Application.Current.GetResource<string>(StringResourceKeys.StrScanningForMusicFiles);
            new CustomDialogWindow(new MusicFileScanView(new MusicFileScanViewModel()),title).ShowDialog();
        }
    }
}
