namespace DJSets.clerks.export
{
    /// <summary>
    /// All classes implementing this interface can export an object of type <see cref="T"/> to a certain
    /// data-storage-location (e.g. Filesystem)
    /// </summary>
    /// <typeparam name="T">The type of data to be exported</typeparam>
    interface IExporter<T>
    {
        #region Functions
        /// <summary>
        /// This function is used to export an element of Type <see cref="T"/>
        /// </summary>
        /// <remarks>
        /// CAUTION: This function is abstractly defined , but in most cases it should
        /// be called in asynchronous code.
        /// </remarks>
        /// <param name="element">The Element that should be exported</param>
        /// <returns>Whether the export operation was successful or not</returns>
        public bool Export(T element);
        #endregion
    }
}
