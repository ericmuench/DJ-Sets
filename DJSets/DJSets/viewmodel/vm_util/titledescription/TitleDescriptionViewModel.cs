using System;
using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.vm_util.titledescription
{
    /// <summary>
    /// This class is the viewmodel for a TitleDescriptionView and provides all necessary data
    /// </summary>
    public class TitleDescriptionViewModel : ValidatableViewModel
    {
        #region Fields
        #region Title
        /// <summary>
        /// This field stores the Title value
        /// </summary>
        private string _title = string.Empty;
        ///<summary>
        /// This property provides the Title-Value stored in <see cref="_title"/> for eventual Binding in xaml.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                Set((value, propName) =>
                {
                    var errMsg = Application.Current.GetResource<string>(StringResourceKeys.StrTitleCannotBeEmptyOrBlank);
                    return SetError(() => !string.IsNullOrWhiteSpace(value), propName, errMsg);
                }, ref _title, value, nameof(Title));
                OnTitleChanged(_title);
            }
        }
        #endregion

        #region Description
        /// <summary>
        /// This field stores the Description value
        /// </summary>
        private string _description = string.Empty;
        ///<summary>
        /// This property provides the Description-Value stored in <see cref="_description"/> for eventual Binding in xaml.
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value, nameof(Description));
        }
        #endregion

        #region OnTitleChanged
        /// <summary>
        /// This function is called every time the title changes to notify other components about the title change
        /// </summary>
        public Action<string> OnTitleChanged = _ => { };

        #endregion

        #endregion
    }
}
