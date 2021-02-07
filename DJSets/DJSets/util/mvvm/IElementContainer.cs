
namespace DJSets.util.mvvm
{
    /// <summary>
    /// This interface defines logic for getting a <see cref="T"/> out of a Data-Structure that holds it, e.g. a viewmodel
    /// </summary>
    public interface IElementContainer<out T>
    {
        #region Functions
        /// <summary>
        /// This function lets the implementations retrieve a <see cref="T"/> from the Data-Structure that implements this Interface
        /// </summary>
        /// <returns>The Element that is contained in the implemented Data-Structure</returns>
        T GetElement();
        #endregion
    }
}
