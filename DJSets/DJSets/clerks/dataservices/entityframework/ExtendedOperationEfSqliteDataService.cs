using System;
using System.Collections.Generic;
using System.Diagnostics;
using DJSets.model.entityframework;
using DJSets.model.model_observe;

namespace DJSets.clerks.dataservices.entityframework
{
    /// <summary>
    /// This class provides an abstract Implementatiion of <see cref="IExtendedModelOperationsDataService{T}"/>
    /// and is a subclass of <see cref="EfSqliteDataService{T}"/> to provide further functionality for a DataService
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ExtendedOperationEfSqliteDataService<T> : EfSqliteDataService<T>, IExtendedModelOperationsDataService<T>
    {
        #region Constructors
        public ExtendedOperationEfSqliteDataService(ModelNotificationCenter notifier) : base(notifier) {}
        #endregion

        #region Functions for IExtendedModelOperations
        /// <see cref="IExtendedModelOperationsDataService{T}.NumberOfElements"/>
        public abstract int NumberOfElements();

        /// <see cref="IExtendedModelOperationsDataService{T}.AddAll"/>
        public abstract bool AddAll(List<T> elements);

        /// <see cref="IExtendedModelOperationsDataService{T}.Clear"/>
        public abstract bool Clear();
        #endregion

        #region Help Functions
        /// <summary>
        /// This function manipulates all data and therefore provides a more abstract implementation of the
        /// Data-Manipulation-Process. 
        /// </summary>
        /// <param name="elements">The Elements to be manipulated in the DB</param>
        /// <param name="onManipulate">A Closure defining the kind of manipulation to execute</param>
        /// <returns>Whether the manipulation was successful or not</returns>
        protected bool ManipulateData(List<T> elements, Action<DjSetsSqliteDbContext, List<T>> onManipulate)
        {
            try
            {
                using var dbCntxt = new DjSetsSqliteDbContext();
                onManipulate.Invoke(dbCntxt, elements);
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
