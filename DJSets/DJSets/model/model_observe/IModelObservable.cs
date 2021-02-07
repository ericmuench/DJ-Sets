using System;

namespace DJSets.model.model_observe
{
    /// <summary>
    /// This Interface defines functions for observing the Model
    /// </summary>
    public interface IModelObservable
    {
        #region Functions
        /// <summary>
        /// This function allows other Classes to register Observer to the Class
        /// implementing this Interface.
        /// </summary>
        /// <param name="observer">Functional Observer to react to be executed when <see cref="NotifyObservers"/> is called</param>
        /// <returns>Whether the register-Operation was successful or not</returns>
        bool RegisterObserver(Action observer);
        
        /// <summary>
        /// This function allows other Classes to unregister Observer from the Class
        /// implementing this Interface.
        /// </summary>
        /// <param name="observer">Functional Observer to react to be executed when <see cref="NotifyObservers"/> is called</param>
        /// <returns>Whether the unregister-Operation was successful or not</returns>
        bool UnRegisterObserver(Action observer);

        /// <summary>
        /// This function is called to notify all observers about a Model Change and exeute them.
        /// </summary>
        void NotifyObservers();
        #endregion
    }
}
