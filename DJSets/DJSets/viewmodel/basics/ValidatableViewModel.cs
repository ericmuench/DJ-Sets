using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DJSets.viewmodel.basics
{
    /// <summary>
    /// This class provides functionality for validating its viewmodel data
    /// </summary>
    /// <remarks>This class implementation was highly inspired by Peter Rill. Therefore LOC from this file do not count</remarks>
    public class ValidatableViewModel : BaseViewModel, INotifyDataErrorInfo
    {

        #region Fields
        /// <summary>
        /// This field stores all errors to all propertys
        /// </summary>
        private readonly Dictionary<string,List<string>> _errors = new Dictionary<string, List<string>>();

        #endregion

        #region Functions
        /// <summary>
        /// This function dies the same as <see cref="Set{T}"/> but with a further validation
        /// </summary>
        protected virtual void Set<T>(Func<T,string,bool> validation, ref T field, T value, [CallerMemberName] string propertyName = "", Action<T> onPropertyChangedCallback = null)
        {
            if ((object)field == (object)value) //copied
                return;//copied
            validation?.Invoke(value, propertyName);//copied
            field = value;//copied
            OnPropertyChanged(propertyName);//copied
            onPropertyChangedCallback?.Invoke(field);
        }

        /// <summary>
        /// This function sets an certain error for a property
        /// </summary>
        /// <param name="isValid">Validation function</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="err">error message</param>
        /// <returns>Value that indicates whether the new property value is valid or not</returns>
        protected bool SetError(Func<bool> isValid, string propertyName, string err)
        {
            if (isValid())
            {
                RemoveError(propertyName,err);
                return true;
            }

            AddError(propertyName, err);
            return false;
        }
        #endregion

        #region Help functions
        /// <summary>
        /// This function adds an error to the <see cref="_errors"/> Dictionary
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="error">Error to be added</param>
        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))//copied
            {
                _errors[propertyName] = new List<string>();//copied
            }

            if (!_errors[propertyName].Contains(error))//copied
            {
                _errors[propertyName].Insert(0, error);//copied
                NotifyOnErrorsChanged(propertyName);//copied
            }
        }

        /// <summary>
        /// This function removes an error from a certain Property in the <see cref="_errors"/>-Dictionary
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="error">Error to be removed</param>
        private void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) && _errors[propertyName].Contains(error))//copied
            {
                _errors[propertyName].Remove(error);//copied
                if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);//copied
                NotifyOnErrorsChanged(propertyName);//copied
            }
        }

        /// <summary>
        /// This function notifies that errors have changed
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyOnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyDataErrorInfo
        /// <inheritdoc cref="INotifyDataErrorInfo"/>
        public IEnumerable GetErrors(string propertyName)
        {
            return (string.IsNullOrEmpty(propertyName) ||
                    !_errors.ContainsKey(propertyName)) ? null : _errors[propertyName];//copied
        }

        /// <summary>
        /// Returns whether there are errors
        /// </summary>
        public virtual bool HasErrors => _errors.Count > 0;//copied

        /// <summary>
        /// This function return if a certain property has errors
        /// </summary>
        /// <param name="propname">The property to check for errors</param>
        /// <returns>if a certain property has errors</returns>
        public bool PropertyHasErrors(string propname)
        {
            var propErrors = _errors.GetValueOrDefault(propname);
            return propErrors != null && propErrors.Count > 0;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

    }
}