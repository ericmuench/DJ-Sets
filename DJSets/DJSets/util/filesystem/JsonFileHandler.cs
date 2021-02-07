using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace DJSets.util.filesystem
{
    /// <summary>
    /// This class loads Objects form a File which is serialized in Json format
    /// </summary>
    public class JsonFileHandler
    {
        #region Functions
        /// <summary>
        /// This function loads a given object of <see cref="T"/> from file
        /// </summary>
        /// <typeparam name="T">The type of object stored in a certain file</typeparam>
        /// <remarks>This operation should be called in parallel code</remarks>
        /// <returns>
        /// The deserialized Object from the file or the default value of <see cref="T"/>
        /// if there is an error
        /// </returns>
        public T GetFromFile<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fileContent = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<T>(fileContent);
                }

                return default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return default;
            }
        }

        /// <summary>
        /// This function stores the given Element to a certain file
        /// </summary>
        /// <typeparam name="T">The type of object stored in a certain file</typeparam>
        /// <param name="element">The element that should be stored</param>
        /// <param name="filePath">The filepath where the element should be stored</param>
        /// <returns>Whether the save-Operation was successful or not</returns>
        /// <remarks>This operation should be called in parallel code</remarks>
        public bool SaveToFile<T>(T element, string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(element);
                File.WriteAllText(filePath,json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        #endregion
    }
}
