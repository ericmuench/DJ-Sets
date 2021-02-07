using System;
using System.IO;
using DJSets.model.settings;
using DJSets.util.async;
using DJSets.util.filesystem;
using DJSets.util.initialization;

namespace DJSets.clerks.filesystem
{
    /// <summary>
    /// This class initializes all file-realated aspects of this app
    /// </summary>
    public class AppFilesInitializer : IInitializer
    {



        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk deals with json files and should init default settings in filesystem
        /// </summary>
        private readonly JsonFileHandler _jsonFileHandler = new JsonFileHandler();

        #endregion

        #region _defaultSettings
        /// <summary>
        /// This field defines the default settings of the app
        /// </summary>
        private readonly AppSettings _defaultSettings = new AppSettings()
        {
            MusicFileScanPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
        };
        #endregion

        #endregion

        #region Functions for IInitializer
        /// <see cref="IInitializer.Init"/>
        public void Init()
        {
            var filePathManager = new DjSetsFilePathManager();
            filePathManager.EnsureApplicationDirectoryExists();
            
            //if there is no settings file yet --> create one with initial settings
            new AsyncTask<bool>().OnExecute(() =>
            {
                if (!File.Exists(filePathManager.ApplicationConfigFilePath()))
                {
                    return _jsonFileHandler
                        .SaveToFile(_defaultSettings, filePathManager.ApplicationConfigFilePath());
                }

                return true;
            }).Start();
        }


        #endregion


    }
}
