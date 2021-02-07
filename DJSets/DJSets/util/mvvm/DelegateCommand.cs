using System;
using System.Windows.Input;
using DJSets.util.async;

namespace DJSets.util.mvvm
{
    /// <summary>
    /// This class defines the base concept of a delegate command, inspired by Implemententation of Peter Rill and slightly modified to
    /// have a Type-Parameter T for which Actions can be executed in a typesafe way. If the command should be used with out a concrete Type, just
    /// use Type object for T. Further a onDone Callback is provided.
    /// </summary>
    /// <typeparam name="T">Type of the Input-Parameter for this DelegateCommand</typeparam>
    /// <typeparam name="TO">Type of the Output-Parameter for this DelegateCommand</typeparam>
    public class DelegateCommand<T,TO> : ICommand
    {

        #region Fields
        /// <summary>
        /// Function to be called when Command is executed
        /// </summary>
        private readonly Func<T,TO> _execute;
        /// <summary>
        /// Function to be called to determine whether a command can be executed or not
        /// </summary>
        private readonly Func<T, bool> _canExecute;
        /// <summary>
        /// Function to be called if "Execute" is Done
        /// </summary>
        private Action<TO> _onDone = delegate {  };

        /// <summary>
        /// Function to be called before "Execute" is executed
        /// </summary>
        private Func<T, bool> _beforeExecute = t => true;

        /// <summary>
        /// This field determines whether the command should be executed by a <see cref="AsyncTask{T}"/> or not
        /// </summary>
        private readonly bool _shouldExecuteAsync;
        #endregion

        #region Constructors
        public DelegateCommand(Func<T,TO> exe, Func<T,bool> canExe = null, bool shouldExecuteAsync = true)
        {
            this._execute = exe;
            this._canExecute = canExe;
            this._shouldExecuteAsync = shouldExecuteAsync;
        }
        #endregion

        #region ICommand 
        /// <summary>
        /// <inheritdoc cref="ICommand"/>
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            var para = default(T);
            if (parameter is T typed)
            {
                para = typed;
            }
            else if (parameter != null)
            {
                return false;
            }

            return  _canExecute(para);
        }

        public void Execute(object parameter)
        {
            T para = default(T);
            if (parameter is T casted)
            {
                para = casted;
            }
            else if (parameter != null)
            {
                //it seems that parameter is not null but is not the correct type either
                //Debug.WriteLine("WARNING: EXECUTE IN DELEGATE COMMAND IS NOT POSSIBLE --> INVALID TYPE");
                throw new ArgumentException("Invalid type in operation");
            }

            if (!CanExecute(parameter) || !_beforeExecute(para)) return;
            
            if (_shouldExecuteAsync)
            {
                new AsyncTask<TO>()
                    .OnExecute(() => _execute(para))
                    .OnDone(result => _onDone(result))
                    .Start();
            }
            else
            {
                var res = _execute(para);
                _onDone(res);
            }
        } 

        public event EventHandler CanExecuteChanged;
        #endregion

        #region Further Functions
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// This method sets an "OnDone"-Callback, e.g. for the UI to be called when <see cref="Execute"/> is finished
        /// </summary>
        /// <param name="action">The Callback to be executed when <see cref="Execute"/> was executed</param>
        public void OnDone(Action<TO> action)
        {
            _onDone = action;
        }

        /// <summary>
        /// This method sets an "BeforeExecute"-Callback, e.g. for the UI to be called when <see cref="Execute"/> is finished
        /// </summary>
        /// <remarks>Use this function to e.g. deny an action shortly before executing</remarks>
        /// <param name="func">The Callback to be executed before <see cref="Execute"/> is executed. If this function returns true, <see cref="Execute"/> is executed else not.</param>
        public void BeforeExecute(Func<T,bool> func)
        {
            _beforeExecute = func;
        }
        #endregion
    }
}
