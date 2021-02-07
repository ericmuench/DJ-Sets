using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;

namespace DJSets.model.export
{
    /// <summary>
    /// This class defines an Enumeration for Export Types of a Setlist
    /// </summary>
    public enum SetlistExportType
    {
        Txt,M3U
    }

    /// <summary>
    /// This class defines extensions and functions for SetlistExportType
    /// </summary>
    public static class SetlistExportTypeUtils
    {
        #region Extensions
        /// <summary>
        /// This extension returns the FileExtensionName for a certain SetlistExportType
        /// </summary>
        /// <param name="exportType">The ExportType to get the FileExtensionName for</param>
        /// <returns>the FileExtensionName of <see cref="exportType"/></returns>
        public static string FileExtensionName(this SetlistExportType exportType)
        {
            return exportType switch
            {
                SetlistExportType.Txt => "txt",
                SetlistExportType.M3U => "m3u",
                _ => SetlistExportTypeDefault().FileExtensionName()
            };
        }

        /// <summary>
        /// This extension provides a note for each ExportType from the string resources
        /// </summary>
        /// <param name="exportType">The EXportType to get the note to</param>
        /// <returns>Note for the ExportType from the string resources</returns>
        public static string ExportNote(this SetlistExportType exportType)
        {
            var app = Application.Current;
            return exportType switch
            {
                SetlistExportType.Txt => app.GetResource<string>(StringResourceKeys.StrExportNoteTxt),
                SetlistExportType.M3U => app.GetResource<string>(StringResourceKeys.StrExportNoteM3U),
                _ => string.Empty
            };
        }
        #endregion

        #region Functions
        /// <summary>
        /// This function shall return all values of the SetlistExportType-Enum in a typesafe way
        /// </summary>
        /// <returns>List of all Music-Keys</returns>
        public static List<SetlistExportType> AllValues()
        {
            var list = new List<SetlistExportType>();
            Enum.GetValues(typeof(SetlistExportType)).CastedAs<SetlistExportType[]>(it =>
            {
                list.AddRange(it);
            });
            return list;
        }

        /// <summary>
        /// This function returns the corresponding SetlistExportType to its fileExtensionName
        /// </summary>
        /// <param name="exName">The file extension name</param>
        /// <param name="defaultVal">The default SetlistExportType if no match was found</param>
        /// <returns>ihe corresponding SetlistExportType to <see cref="exName"/></returns>
        public static SetlistExportType GetByFileExtensionName(string exName, SetlistExportType defaultVal = SetlistExportType.Txt)
        {
            var fileExtensionNames = 
                AllValues()
                .Select(it => new Tuple<SetlistExportType,string>(it, it.FileExtensionName()));

            foreach (var (type, fileExtensionName) in fileExtensionNames)
            {
                if (fileExtensionName == exName)
                {
                    return type;
                }
            }

            return defaultVal;
        }


        /// <summary>
        /// This function returns the default value for the SetlistExportType-Enum
        /// </summary>
        public static SetlistExportType SetlistExportTypeDefault() => SetlistExportType.Txt;
        #endregion
    }
}
