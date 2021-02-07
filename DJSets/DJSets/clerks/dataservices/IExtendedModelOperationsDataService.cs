using System.Collections.Generic;

namespace DJSets.clerks.dataservices
{
    /// <summary>
    /// This interface provides further functionalities for interacting with Model Data
    /// </summary>
    /// <typeparam name="T">Type of data to interact with</typeparam>
    public interface IExtendedModelOperationsDataService<T> : IDataService<T>
    {
        #region Functions
        /// <summary>
        /// This function returns how many Model-Elements are saved
        /// </summary>
        /// <returns>Number of saved Elements</returns>
        int NumberOfElements();

        /// <summary>
        /// This function adds all elements of <see cref="elements"/> to the persistence layer
        /// </summary>
        /// <param name="elements">elements to be added</param>
        /// <returns>Whether this operation was successful or not</returns>
        bool AddAll(List<T> elements);

        /// <summary>
        /// This function deletes all entries
        /// </summary>
        /// <returns>Whether the clear-Operation was successful or not</returns>
        bool Clear();

        #endregion
    }
}
