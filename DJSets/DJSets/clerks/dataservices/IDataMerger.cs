using System.Collections.Generic;

namespace DJSets.clerks.dataservices
{

    /// <summary>
    /// This interface defines logic for merging new data into existing data
    /// </summary>
    /// <typeparam name="T">The Type of Data to be merged</typeparam>
    interface IDataMerger<T>
    {
        #region Functions
        /// <summary>
        /// This function merges Data into existing data structures
        /// </summary>
        /// <param name="elements">the new elements to be merged</param>
        /// <param name="shouldRemoveOldData">
        /// This parameter determines whether old data that is not contained in the new data
        /// should be deleted or not
        /// </param>
        /// <returns>Whether the merge operation was successful</returns>
        bool Merge(List<T> elements, bool shouldRemoveOldData);

        #endregion
    }
}
