using System.Text.RegularExpressions;

namespace DJSets.util.mvvm.validation
{
    /// <summary>
    /// This class is able to validate a tempo value in BPM
    /// </summary>
    public class TempoValidator : IValidator<string>
    {
        /// <summary>
        /// This field defines a regex about which BPM-Values are valid
        /// </summary>
        private readonly Regex _tempoRegex = new Regex("^(([1-5]?[1-9]?|[1-5][0-9]?)[0-9])$");

        #region Interface functions for IValidator
        /// <see cref="IValidator{T}.IsValid"/>
        public bool IsValid(string tempo) => _tempoRegex.IsMatch(tempo);
        #endregion
    }
}
