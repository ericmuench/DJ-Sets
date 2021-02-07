using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DJSets.Annotations;

namespace DJSets.viewmodel.basics
{
    /// <summary>
    /// This class is the base class for all Viewmodels implementing the Basic Operations which can be done on all ViewModels
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {

        #region Functions

        /// <summary>
        ///     This Function can set the value of a field and automatically call "OnPropertyChanged"
        /// </summary>
        /// <remarks>This function was copied from Lecture Slides of Peter Rill and slightly modified (--> -4 LOC)</remarks>
        /// <typeparam name="T">Type of the variable</typeparam>
        /// <param name="field">Field where the value is stored</param>
        /// <param name="value">new value</param>
        /// <param name="propertyName">Name of the Property</param>
        /// <param name="onPropertyChangedCallback">Callback to be called when Set-Operation was completed</param>
        protected virtual void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "", Action<T> onPropertyChangedCallback = null)
        {
            if ((object)field == (object)value) //copied
                return;//copied
            field = value;//copied
            OnPropertyChanged(propertyName);//copied
            onPropertyChangedCallback?.Invoke(field);
        }
        #endregion


        #region INotifyPropertyChanged
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This event exists because of the Implementation of "INotifyPropertyChanged" and is invoked to notify that data has been changed
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
