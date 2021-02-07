using System;
using System.Collections.Generic;
using System.Diagnostics;
using DJSets.model.entityframework;
using DJSets.model.model_observe;

namespace DJSets.clerks.dataservices.entityframework
{
    /// <summary>
    /// This class defines abstract logic to interact with persistence data
    /// in Form of an SQLite-DB and implementing the <see cref="IDataService{T}"/>
    /// interface
    /// </summary>
    /// <typeparam name="T">The type of data to interact with.</typeparam>
    public abstract class EfSqliteDataService<T> : IDataService<T>
    {
        #region Constructors
        public EfSqliteDataService(ModelNotificationCenter notifier)
        {
            NotificationCenter = notifier;
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the NotificationCenter to be informed about Model-Changes
        /// </summary>
        protected readonly ModelNotificationCenter NotificationCenter;

        #endregion

        #region Interface Functions for IDataService declared as abstract
        /// <see cref="IDataService{T}.Add(T)"/>
        public abstract bool Add(T element);

        /// <see cref="IDataService{T}.Remove(T)"/>
        public abstract bool Remove(T element);

        /// <see cref="IDataService{T}.GetAll"/>
        public abstract List<T> GetAll();

        /// <see cref="IDataService{T}.Update(T)"/>
        public abstract bool Update(T element);
        #endregion

        #region Help Functions
        /// <summary>
        /// This function manipulates data and therefore provides a more abstract implementation of the
        /// Data-Manipulation-Process. 
        /// </summary>
        /// <param name="element">The Element to be manipulated in the DB</param>
        /// <param name="onManipulate">A Closure defining the kind of manipulation to execute</param>
        /// <returns>Whether the manipulation was successful or not</returns>
        protected bool ManipulateData(T element, Action<DjSetsSqliteDbContext, T> onManipulate)
        {
            try
            {
                using var dbCntxt = new DjSetsSqliteDbContext();
                onManipulate.Invoke(dbCntxt, element);
                var writtenEntryCount = dbCntxt.SaveChanges();
                NotificationCenter.NotifyObservers();
                return writtenEntryCount >= 1; // This indicates that at least one DB-Entry was modified
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        #endregion


    }
}
