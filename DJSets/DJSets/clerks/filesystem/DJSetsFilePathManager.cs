using System;
using System.IO;

namespace DJSets.clerks.filesystem
{
    /// <summary>
    /// This class manages file paths for the application
    /// </summary>
    public class DjSetsFilePathManager
    {
        #region Functions
        /// <summary>
        /// This function returns the path where the application data should be stored
        /// </summary>
        /// <returns></returns>
        public string ApplicationDataDirectory() 
            => $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\DJSets\\ApplicationData";

        /// <summary>
        /// This function return the path to the applications SQLite DB
        /// </summary>
        /// <returns>the path to the applications SQLite DB</returns>
        public string ApplicationDbPath() => $"{ApplicationDataDirectory()}\\djsets.db";

        /// <summary>
        /// This function return the path to the applications Config File
        /// </summary>
        /// <returns>the path to the applications Config File</returns>
        public string ApplicationConfigFilePath() => $"{ApplicationDataDirectory()}\\djsets_appconfig.json";

        /// <summary>
        /// This function ensures that the directory for storing data does exist.
        /// If it does not already exist, it will be created.
        /// </summary>
        public void EnsureApplicationDirectoryExists()
        {
            //db should be located in ApplicationData-Directory
            var appDir = ApplicationDataDirectory();
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }
        }
        #endregion
    }
}
