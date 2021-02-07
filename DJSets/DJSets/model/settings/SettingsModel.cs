
namespace DJSets.model.settings
{
    /// <summary>
    /// This class stores all values of the settings of the app which will be persisted
    /// </summary>
    public class AppSettings
    {
        #region Fields
        /// <summary>
        /// This field stores the path to the root folder for scanning files
        /// for getting music info from computer
        /// </summary>
        public string MusicFileScanPath { get; set; }
        #endregion
    }
}
