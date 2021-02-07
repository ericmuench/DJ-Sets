using System;
using System.Diagnostics;
using System.IO;
using DJSets.viewmodel.setlist.setlist_export;

namespace DJSets.util.mvvm.validation
{
    /// <summary>
    /// This Validator class that conforms to <see cref="IValidator{T}"/> can validate a <see cref="SetlistExportViewModel"/>
    /// </summary>
    class SetlistExportViewModelValidator : IValidator<SetlistExportViewModel>
    {
        #region Functions
        /// <summary>
        /// This function checks whether the Filename in context of a <see cref="SetlistExportViewModel"/> is valid or not
        /// </summary>
        /// <param name="fileName">the filename that should be checked</param>
        /// <returns>whether the Filename in context of a <see cref="SetlistExportViewModel"/> is valid or not</returns>
        public bool IsValidFileName(string fileName) => !string.IsNullOrWhiteSpace(fileName);

        /// <summary>
        /// This function checks whether the directory-path is valid meaning if it is an real existing directory on the computer 
        /// </summary>
        /// <param name="dir">The directory that should be checked for existence</param>
        /// <returns>whether the directory-path is valid meaning if it is an real existing directory on the computer</returns>
        public bool IsValidDirectoryPath(string dir)
        {
            try
            {
                return Directory.Exists(dir);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        #endregion

        #region Interface functions for IValidator
        /// <see cref="IValidator{T}.IsValid"/>
        public bool IsValid(SetlistExportViewModel element)
        {
            // if the vm has errors then it can not be valid
            if (element.HasErrors)
            {
                return false;
            }

            //this checks the values of element separately even if there are no error, e.g. in Initial Config.
            return IsValidFileName(element.FileName) && IsValidDirectoryPath(element.DirectoryPath);
        }
        #endregion
    }
}
