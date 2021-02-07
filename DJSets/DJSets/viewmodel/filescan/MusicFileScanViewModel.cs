using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using DJSets.clerks.dataservices;
using DJSets.clerks.dataservices.entityframework;
using DJSets.clerks.dataservices.filesystem;
using DJSets.clerks.filesystem;
using DJSets.model.entityframework;
using DJSets.model.settings;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.filescan
{
    /// <summary>
    /// This ViewModel defines  logic for a view that can scan the filesystem for music files
    /// </summary>
    public class MusicFileScanViewModel : BaseViewModel
    {
        #region Constructors
        public MusicFileScanViewModel()
        {
            ConfigCommands();

            var dbDataService = new EfSqliteSongDataService();
            _songDataService = dbDataService;
            _songDataMerger = dbDataService;

            ConfigBackgroundWorker();
        }
        #endregion

        #region Commands
        public DelegateCommand<object,bool> CancelCommand { get; set; }

        /// <summary>
        /// This command defines save action
        /// </summary>
        public DelegateCommand<object,bool> SaveCommand { get; set; }

        #endregion

        #region Background Worker
        /// <summary>
        /// This field can execute a certain task in the background and gives ui feedback via certain events
        /// </summary>
        /// <see cref="BackgroundWorker"/>
        public readonly BackgroundWorker BackgroundWorker = new BackgroundWorker();

        #endregion

        #region Clerks
        /// <summary>
        /// This clerk provides access to the app settings
        /// </summary>
        private readonly IDataService<AppSettings> _appSettingsDataService = new AppSettingsDataService();

        /// <summary>
        /// This clerk scans all Music Files and returns all songs that he  has found
        /// </summary>
        private readonly MusicFileScanner _musicFileScanner = new MusicFileScanner();

        /// <summary>
        /// This clerk provides the possibility to add multiple songs to persistence layer
        /// </summary>
        private readonly IExtendedModelOperationsDataService<Song> _songDataService;

        /// <summary>
        /// This clerk allows to merge Data into the existing amount of songs
        /// </summary>
        private readonly IDataMerger<Song> _songDataMerger;
        #endregion

        #region Fields

        #region ShouldOverrideSongDb
        /// <summary>
        /// This field stores the ShouldOverrideSongDb value
        /// </summary>
        private bool _shouldOverrideSongDb;
        ///<summary>
        /// This property provides the ShouldOverrideSongDb-Value stored in <see cref="_shouldOverrideSongDb"/> for eventual Binding in xaml.
        /// </summary>
        public bool ShouldOverrideSongDb
        {
            get => _shouldOverrideSongDb;
            set => Set(ref _shouldOverrideSongDb, value, nameof(ShouldOverrideSongDb));
        }
        #endregion

        #region ScannedSongs
        /// <summary>
        /// This field stores the ScannedSongs value
        /// </summary>
        private List<Song> _scannedSongs;
        ///<summary>
        /// This property provides the ScannedSongs-Value stored in <see cref="_scannedSongs"/> for eventual Binding in xaml.
        /// </summary>
        public List<Song> ScannedSongs
        {
            get => _scannedSongs;
            set => Set(ref _scannedSongs, value, nameof(ScannedSongs));
        }
        #endregion

        #endregion

        #region Events

        /// <summary>
        /// This Action is called when there is an error while scanning the file system
        /// </summary>
        public Action<Exception> OnScanError = _ => { };

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures all commands
        /// </summary>
        private void ConfigCommands()
        {
            //Cancel Command
            CancelCommand = new DelegateCommand<object, bool>(_ =>
            {
                if (!BackgroundWorker.IsBusy)
                {
                    return true;
                }

                try
                {
                    BackgroundWorker.CancelAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }

            }, _ => !BackgroundWorker.CancellationPending);

            //Save Command
            SaveCommand = new DelegateCommand<object, bool>(_ =>
            {
                if (ShouldOverrideSongDb && _songDataService.NumberOfElements() > 0)
                {
                    return _songDataService.Clear() && _songDataService.AddAll(ScannedSongs);
                }

                return _songDataMerger.Merge(ScannedSongs,false);
            }, _ => _scannedSongs != null && !BackgroundWorker.IsBusy);
        }

        #region BackgroundWorker

        /// <summary>
        /// This function configures <see cref="BackgroundWorker"/>
        /// </summary>
        private void ConfigBackgroundWorker()
        {
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = true;
            BackgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            BackgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        /// <summary>
        /// This function handles the event when <see cref="BackgroundWorker"/> is completed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event Args</param>
        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //react to result
            e.Result.CastedAs<Tuple<List<Song>, Exception>>(result =>
            {
                Debug.WriteLine($"Result is ({result.Item1},{result.Item2})");
                result.Item1.NotNull(songs =>
                {
                    ScannedSongs = songs;
                });

                result.Item2.NotNull(OnScanError);
            });


            //update commands
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// This function defines the code, that <see cref="BackgroundWorker"/> has to execute in background
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event Args</param>
        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            _appSettingsDataService
                .GetAll()
                .FirstOrDefault().NotNull(appSettings =>
                {
                    try
                    {
                        var scanPath = appSettings.MusicFileScanPath;
                        var scSongs = _musicFileScanner.StartScan(BackgroundWorker, scanPath);
                        e.Result = new Tuple<List<Song>, Exception>(scSongs, null);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                        e.Result = new Tuple<List<Song>,Exception>(null,exception);
                    }
                });
        }
        #endregion

        #endregion
    }
}
