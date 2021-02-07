using System.Collections.Generic;
using DJSets.clerks.filesystem;
using DJSets.model.settings;
using DJSets.util.Extensions;
using DJSets.util.filesystem;

namespace DJSets.clerks.dataservices.filesystem
{
    /// <summary>
    /// This class defines logic for persisting Apps settings Settings
    /// </summary>
    public class AppSettingsDataService : IDataService<AppSettings>
    {
        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk handles saving data to filesystem or loading data from it
        /// </summary>
        private readonly JsonFileHandler _jsonFileHandler = new JsonFileHandler();

        #endregion

        #region Cached Instance
        /// <summary>
        /// This field stores an instance of <see cref="AppSettings"/> to
        /// make read-only access more performant
        /// </summary>
        private AppSettings _appSettings;
        #endregion

        #endregion

        #region Functions for IDataService
        /// <see cref="IDataService{T}.Add"/>
        /// <remarks>
        /// Not implemented --> there should only be one stored object of AppSettings generated automatically
        /// when App is initialized
        /// </remarks>
        public bool Add(AppSettings element)
        {
            return false;
        }

        /// <see cref="IDataService{T}.Remove"/>
        /// <remarks>
        /// Not implemented --> there should only be one stored object of AppSettings generated automatically
        /// when App is initialized
        /// </remarks>
        public bool Remove(AppSettings element) => false;

        /// <see cref="IDataService{T}.GetAll"/>
        /// <remarks>
        /// This function always returns a List with only one Element due to the fact that
        /// there can only be one settings
        /// </remarks>
        public List<AppSettings> GetAll()
        {
            if (_appSettings == null)
            {
                var filePathManager = new DjSetsFilePathManager();
                //lazy load settings
                _appSettings = _jsonFileHandler.GetFromFile<AppSettings>(filePathManager.ApplicationConfigFilePath());
            }
            
            return new List<AppSettings>().Apply((it) => it.Add(_appSettings));
        }

        /// <see cref="IDataService{T}.Update"/>
        public bool Update(AppSettings element)
        {
            var filePathManager = new DjSetsFilePathManager();
            var success = _jsonFileHandler.SaveToFile(
                element,
                filePathManager.ApplicationConfigFilePath()
            );

            if (success)
            {
                _appSettings = element;
            }

            return success;
        }
        #endregion
    }
}
