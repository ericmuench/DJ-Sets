
namespace DJSets.util.mvvm.validation
{
    /// <summary>
    /// This interface defines logic for validating data
    /// </summary>
    /// <typeparam name="T">The Type of data to be validated</typeparam>
    interface IValidator<T>
    {
        /// <summary>
        /// This function returns if <see cref="element"/> is valid
        /// </summary>
        /// <param name="element">the element that should be validated</param>
        /// <returns>Whether <see cref="element"/> is valid or not</returns>
        bool IsValid(T element);
    }
}
