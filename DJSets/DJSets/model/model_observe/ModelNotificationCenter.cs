using System;
using System.Collections.Generic;

namespace DJSets.model.model_observe
{
    /// <summary>
    /// This Class defines the NotificationCenter for Model-Changes. Other classes, e.g. ViewModels
    /// can register certain actions to react to changes in the Model.
    /// </summary>
    public class ModelNotificationCenter : IModelObservable
    {
        #region Constructors
        /// <summary>
        /// Private Constructor for Singleton-Pattern
        /// </summary>
        private ModelNotificationCenter(){}
        #endregion

        #region Static Fields
        /// <summary>
        /// This static Singleton-Instance is responsible for notifying all its Observers when there are Changes on
        /// Songs
        /// </summary>
        public static ModelNotificationCenter SongNotifier = new ModelNotificationCenter();
        /// <summary>
        /// This static Singleton-Instance is responsible for notifying all its Observers when there are Changes on
        /// Setlists
        /// </summary>
        public static ModelNotificationCenter SetlistNotifier = new ModelNotificationCenter();
        #endregion

        #region Fields
        /// <summary>
        /// This field stores all observers that are registered
        /// </summary>
        private readonly List<Action> _observers = new List<Action>();
        #endregion

        #region Interface Functions for IModelObservable
        /// <see cref="IModelObservable.RegisterObserver"/>
        public bool RegisterObserver(Action observer)
        {
            var obSize = _observers.Count;
            _observers.Add(observer);
            return _observers.Count > obSize;
        }

        /// <see cref="IModelObservable.UnRegisterObserver"/>
        public bool UnRegisterObserver(Action observer) => _observers.Remove(observer);

        /// <see cref="IModelObservable.NotifyObservers"/>
        public void NotifyObservers()
        {
            foreach (var obs in _observers)
            {
                obs.Invoke();
            }
        }
        #endregion
    }
}
