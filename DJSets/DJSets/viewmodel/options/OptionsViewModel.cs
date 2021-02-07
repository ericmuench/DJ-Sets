
using System.IO;
using System.Linq;
using System.Windows;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.filesystem;
using DJSets.model.settings;
using DJSets.resources;
using DJSets.util.async;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.options
{
    /// <summary>
    /// This class is the viewmodel for the "OptionsMenuView"
    /// </summary>
    public class OptionsViewModel : ValidatableViewModel
    {
        #region Constructors

        public OptionsViewModel()
        {
            LoadData();
            ConfigCommands();
        }

        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk provides access to AppSettings
        /// </summary>
        private readonly IDataService<AppSettings> _dataService = new AppSettingsDataService();

        #endregion

        #region _appSettings
        /// <summary>
        /// This field provides access to the settings of the app
        /// </summary>
        private AppSettings _appSettings;

        #endregion

        #region ScanPath

        /// <summary>
        /// This field defines the path for the music file scan
        /// </summary>
        private string _scanPath = string.Empty;
        /// <summary>
        /// This property provides the stored value defined in <see cref="_scanPath"/> for binding.
        /// </summary>
        public string ScanPath
        {
            get => _scanPath;
            set => Set((val,propName) =>
            {
                var errMsg =
                    Application.Current.GetResource<string>(StringResourceKeys.StrDirectoryPathCanOnlyBeExistingFile);
                var errSet = SetError(() => Directory.Exists(val), propName, errMsg);
                OnPropertyChanged(nameof(CanScanMusicFiles));
                return errSet;
            }, ref _scanPath, value, nameof(ScanPath),_ => NotifyValueChangeToCommands());
        }

        /// <summary>
        /// This property checks whether <see cref="ScanPath"/> has errors. If yes, a filescan is impossible
        /// </summary>
        public bool CanScanMusicFiles => !PropertyHasErrors(nameof(ScanPath));
        #endregion

        #endregion

        #region Commands
        /// <summary>
        /// This command handles saving the Scan Path for Music files stored in <see cref="ScanPath"/>
        /// </summary>
        public DelegateCommand<object,bool> SaveMusicScanPathCommand { get; set; }

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures this viewmodels commands
        /// </summary>
        private void ConfigCommands()
        {
            //SaveMusicScanPathCommand
            SaveMusicScanPathCommand = new DelegateCommand<object, bool>(_ =>
            {
                _appSettings.MusicFileScanPath = ScanPath;
                return _dataService.Update(_appSettings);
            }, _ => Directory.Exists(ScanPath));
            
        }

        /// <summary>
        /// This function asynchronously loads the data for <see cref="_appSettings"/> and applies it to them
        /// </summary>
        private void LoadData()
        {
            new AsyncTask<AppSettings>()
                .OnExecute(() => _dataService.GetAll()?.FirstOrDefault())
                .OnDone(result =>
                {
                    result.NotNull(settings =>
                    {
                        _appSettings = settings;
                        ApplyData();
                    });
                }).Start();
        }

        /// <summary>
        /// This function applies settingsData to VM fields
        /// </summary>
        private void ApplyData() => _appSettings.NotNull(settings =>
        {
            ScanPath = settings.MusicFileScanPath;
        });

        /// <summary>
        /// This Function informs all command that some data has changed and that they should check whether
        /// they can be executed or not.
        /// </summary>
        private void NotifyValueChangeToCommands()
        {
            SaveMusicScanPathCommand.NotifyCanExecuteChanged();
        }



        #endregion
    }
}
