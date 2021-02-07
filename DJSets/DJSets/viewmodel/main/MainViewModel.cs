
using DJSets.clerks.ef_util;
using DJSets.clerks.filesystem;
using DJSets.util.initialization;
using DJSets.view;

namespace DJSets.viewmodel.main
{
    /// <summary>
    /// This ViewModel is responsible for all App-Specific Actions due to the fact that it is hosted
    /// by <see cref="MainWindow"/>
    /// </summary>
    public class MainViewModel : IInitializer
    {
        #region Fields
        /// <summary>
        /// This class has logic for initializing the used DB
        /// </summary>
        private readonly IInitializer _dbInitializer = new DbInitializer();

        /// <summary>
        /// This clerk initializes the files needed for this app
        /// </summary>
        private readonly IInitializer _fileInitializer = new AppFilesInitializer();
        #endregion

        #region Functions for IInitializer
        /// <summary>
        /// This function should Initialize all relevant components of the app
        /// </summary>
        /// <see cref="IInitializer.Init"/>
        public void Init()
        {
            _fileInitializer.Init();
            _dbInitializer.Init();
        }


        #endregion


    }
}
