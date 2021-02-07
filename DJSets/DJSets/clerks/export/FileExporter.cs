
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DJSets.clerks.export
{
    /// <summary>
    /// This class defines a class that is able to export some data of type <see cref="T"/> to a file
    /// </summary>
    /// <typeparam name="T">The type of data to be exported to a file</typeparam>
    public abstract class FileExporter<T> : IExporter<T>
    {
        #region Constructors
        protected FileExporter(string filePath)
        {
            FilePath = filePath;
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the absolute filepath to where data should be exported
        /// </summary>
        protected readonly string FilePath;
        #endregion

        #region Functions
        /// <summary>
        /// This function returns the encoding for writing into files. Usually it is the default encodingbut
        /// it can also overridden by subclasses to change the encoding
        /// </summary>
        /// <returns>the encoding for writing into files</returns>
        protected virtual Encoding GetEncoding() => Encoding.Default; 

        #endregion

        #region Interface Functions for IExporter
        /// <summary>
        /// This class implements the Export Function as a Template Pattern by calling the
        /// <see cref="ProvideFileContent"/>-Method to get the files content and then write it into the file.
        /// </summary>
        /// <see cref="IExporter{T}.Export(T)"/>
        public bool Export(T element)
        {
            try
            {
                File.WriteAllText(FilePath, ProvideFileContent(element),GetEncoding());
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        #endregion

        #region Abstract Functions
        /// <summary>
        /// This function provides the export-files content and should be implemented by subclasses.
        /// </summary>
        /// <param name="element">The element to get the file content from</param>
        /// <returns>the export-files content</returns>
        protected abstract string ProvideFileContent(T element);
        #endregion
    }
}
