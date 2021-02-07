using System.Diagnostics;
using DJSets.clerks.filesystem;
using DJSets.model.entityframework;
using DJSets.util.initialization;

namespace DJSets.clerks.ef_util
{
    /// <summary>
    /// This class defines all logic for initializing the applications database
    /// </summary>
    public class DbInitializer : IInitializer
    {
        #region Functions for the IInitializer Interface
        /// <summary>
        /// This function takes care of all initializations of the db
        /// </summary>
        /// <see cref="IInitializer.Init"/>
        public void Init()
        {
            using (var dbCntxt = new DjSetsSqliteDbContext())
            {
                //dbCntxt.Database.EnsureDeleted();
                new DjSetsFilePathManager().EnsureApplicationDirectoryExists();
                dbCntxt.Database.EnsureCreated();
                dbCntxt.SaveChanges();
                Debug.WriteLine("DB Init finished");
            }
        }
        #endregion

    }
}
