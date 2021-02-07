using System.Collections.Generic;

namespace DJSets.clerks.dataservices
{
    /// <summary>
    /// This interface declares logic for providing CRUD-Operations to data in an abstract way
    /// </summary>
    /// <typeparam name="T">Type of data the DataService is providing CRUD-Operations to</typeparam>
    public interface IDataService<T>
    {
        #region Functions
        /// <summary>
        /// This abstract function declares the add operation for data
        /// </summary>
        /// <param name="element">element to be added</param>
        /// <returns>Whether the Add-Operation was successful or not</returns>
        public bool Add(T element);

        /// <summary>
        /// This abstract function declares the remove operation for data
        /// </summary>
        /// <param name="element">element to be removed</param>
        /// <returns>Whether the Remove-Operation was successful or not</returns>
        public bool Remove(T element);

        /// <summary>
        /// This abstract function declares the read operation for all data of the certain type
        /// </summary>
        /// <returns>The element from data data source</returns>
        public List<T> GetAll();

        /// <summary>
        /// This abstract function declares the update operation for data
        /// </summary>
        /// <param name="element">element to be updated</param>
        /// <returns>Whether the Update-Operation was successful or not</returns>
        public bool Update(T element);
        #endregion
    }
}
