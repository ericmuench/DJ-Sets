using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.setlist.setlist_export;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DJSets.view.setlist.setlist_export
{
    /// <summary>
    /// This class defines code behind logic for a SetlistExportView
    /// </summary>
    public partial class SetlistExportView : HostedPage
    {
        #region Constructors
        public SetlistExportView(SetlistExportViewModel vm)
        {
            InitializeComponent();
            _viewModel = vm;
            DataContext = _viewModel;
            ConfigureViewModelCommands();
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the ViewModel for this view
        /// </summary>
        private readonly SetlistExportViewModel _viewModel;
        #endregion

        /// <summary>
        /// This function opens a FileDialog to select a directory
        /// </summary>
        /// <param name="sender">The sender of the Event</param>
        /// <param name="e">Event Args</param>
        private void OnSelectExportDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog().Apply(it =>
            {
                it.IsFolderPicker = true;
                it.Multiselect = false;
                
            });
            
            var result = dialog.ShowDialog();
            Host.Focus();
            if (result == CommonFileDialogResult.Ok)
            {
                _viewModel.DirectoryPath = dialog.FileName;
            }
        }

        #region Help Functions
        /// <summary>
        /// This function adds further UI Callbacks to VM-Commands
        /// </summary>
        private void ConfigureViewModelCommands()
        {
            _viewModel.CancelCommand.OnDone(success =>
            {
                Host.DialogResult = success;
                Host.Close();
            });

            _viewModel.ExportCommand.OnDone(success =>
            {
                if (success)
                {
                    var app = Application.Current;
                    var title = app.GetResource<string>(StringResourceKeys.StrNote);
                    var msg = app.GetResource<string>(StringResourceKeys.StrTemplateSetlistExportSuccess)
                        .Replace("#", $"\n\n{_viewModel.GetAbsoluteFilePath()}\n\n");
                    WpfUiExtensions.ShowMessageBox(title, msg, MessageBoxButton.OK, MessageBoxImage.Information);
                    Host.DialogResult = true;
                    Host.Close();
                }
                else
                {
                    this.ShowMessageBoxWithResources(StringResourceKeys.StrNote,
                        StringResourceKeys.StrExportSetlistFailed,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        #endregion
    }
}
