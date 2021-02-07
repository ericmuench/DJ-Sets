using DJSets.clerks.timeformat;
using DJSets.viewmodel.setlist.setlist_autogenerate;

namespace DJSets.util.mvvm.validation
{
    /// <summary>
    /// This class validates fields of a SetlistAutogenerateViewModel 
    /// </summary>
    public class SetlistAutogenerateViewModelValidator : IValidator<SetlistAutogenerateViewModel>
    {
        #region Fields
        /// <summary>
        /// This class validates the time value
        /// </summary>
        private readonly IValidator<string> _lengthValidator = new TimeFormatConverter();


        /// <summary>
        /// this field validates the tempo value
        /// </summary>
        private readonly IValidator<string> _tempoValidator = new TempoValidator();

        #endregion

        #region Functions
        /// <summary>
        /// This function validates the tempo value
        /// </summary>
        /// <param name="tempo">The tempo value that should be validated</param>
        /// <returns>Whether <see cref="tempo"/> is valid or not</returns>
        public bool IsValidTempo(string tempo) => _tempoValidator.IsValid(tempo);

        /// <summary>
        /// This function validates the setlists length value
        /// </summary>
        /// <param name="length">The length value that should be validated</param>
        /// <returns>Whether <see cref="length"/> is valid or not</returns>
        public bool IsValidLength(string length) => _lengthValidator.IsValid(length);

        /// <summary>
        /// This function returns whether <see cref="vm"/> has valid Autogeneration Parameters meaning Length and Tempo
        /// but not title due to the fact that the title is only relevant for saving but not for autogeneration.
        /// </summary>
        /// <param name="vm">The ViewModel to be checked</param>
        /// <returns>whether <see cref="vm"/> has valid Autogeneration Parameters meaning Length and Tempo
        /// but not title</returns>
        public bool IsValidAutogenerationParameters(SetlistAutogenerateViewModel vm) 
            => IsValidLength(vm.SetlistLength) && IsValidTempo(vm.StartTempo);
        #endregion


        #region Interface functions for IValidator
        /// <see cref="IValidator{T}.IsValid(T)"/>
        public bool IsValid(SetlistAutogenerateViewModel vm)
        {
            //quick validation --> if vm has errors it can not be valid
            if (vm.HasErrors)
            {
                return false;
            }

            /*at this point vm might not have any registered errors but maybe it has its initial values
             which may not be valid but also not registered as errors --> make a second check over all fields
            that need validation
             */

            return !string.IsNullOrWhiteSpace(vm.Title)
                   && IsValidAutogenerationParameters(vm);
        }
        #endregion
    }
}
