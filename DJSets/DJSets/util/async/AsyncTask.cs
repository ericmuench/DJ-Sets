using System;
using System.Threading.Tasks;
using System.Windows;
using DJSets.util.Extensions;

namespace DJSets.util.async
{
    /// <summary>
    /// This class defines a Task that can execute some code asynchronously and return the result into a OnDone-Callback to be executed
    /// when the task is finished. The AsyncTask uses a FluentAPI-Syntax
    /// </summary>
    /// <typeparam name="T">Type of the Result of the task being executed asynchronously.</typeparam>
    public class AsyncTask<T>
    {
        #region Fields
        /// <summary>
        /// This field stores the Function that will be executed asynchronously when <see cref="Start"/> is called
        /// </summary>
        private Func<T> _onExecute;
        /// <summary>
        /// This field stores the Action that is executed when execution if <see cref="_onExecute"/> in <see cref="Start"/> is finished
        /// </summary>
        private Action<T> _onDone;
        #endregion

        #region Functions
        /// <summary>
        /// This function sets the <see cref="_onExecute"/>-Value
        /// </summary>
        /// <param name="exe">The Function that should be executed asynchronously</param>
        /// <returns>this AsyncTask to guarantee FluentAPI-Syntax</returns>
        public AsyncTask<T> OnExecute(Func<T> exe)
        {
            exe.NotNull(it => _onExecute = exe);
            return this;
        }

        /// <summary>
        /// This function sets the <see cref="_onDone"/>-Value
        /// </summary>
        /// <param name="done">The Action that should be executed when <see cref="_onExecute"/> is finished</param>
        /// <returns>this AsyncTask to guarantee FluentAPI-Syntax</returns>
        public AsyncTask<T> OnDone(Action<T> done)
        {
            done.NotNull(it => _onDone = done);
            return this;
        }

        /// <summary>
        /// This function starts the asynchronous execution of <see cref="_onExecute"/> and calls <see cref="_onDone"/>
        /// as soon as <see cref="_onExecute"/> is finished.
        /// </summary>
        /// <remarks>
        /// This function only works if <see cref="_onExecute"/> is not null and is necessary to be called.
        /// Otherwise, the in <see cref="OnExecute"/> defined Function is not executed.
        /// </remarks>
        public void Start()
        {
            _onExecute.NotNull(exe =>
            {
                var task = Task.Run(exe);
                var awaiter = task.GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _onDone?.Invoke(awaiter.GetResult());
                    });
                });
            });
        }
        #endregion
    }
}
