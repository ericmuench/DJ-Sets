using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DJSets.clerks.ef_util;
using DJSets.model.entityframework;
using DJSets.model.model_observe;
using Microsoft.EntityFrameworkCore;

namespace DJSets.clerks.dataservices.entityframework
{
    /// <summary>
    /// This class is responsible for all DatabaseOperations for SetlistPositions of a certain Setlist
    /// </summary>
    /// <remarks>
    /// A EfSqliteSetlistPositionDataService always acts in context of a Setlist and therefore does not
    /// affect all setlistpositions globally if not documented explicitly
    /// </remarks>
    class EfSqliteSetlistPositionDataService : 
        ExtendedOperationEfSqliteDataService<SetlistPosition>,
        ISetlistPositionMover
    {
        #region Constructors
        public EfSqliteSetlistPositionDataService(Setlist setlist) : base(ModelNotificationCenter.SetlistNotifier)
        {
            _setlist = setlist;
        }
        #endregion

        #region Fields

        #region Clerks
        /// <summary>
        /// This clerk handles further functionality of updating the the SetlistPosition Position-Values
        /// </summary>
        private readonly SetlistPositionPositionUpdater _positionUpdater = new SetlistPositionPositionUpdater();
        #endregion

        #region _setlist
        /// <summary>
        /// This field is necessary to get access to a certain <see cref="Setlist"/> to where the SetlistPositions belong
        /// </summary>
        private Setlist _setlist;
        #endregion


        #endregion

        #region Functions from IDataService
        /// <see cref="IDataService{T}.Add"/>
        public override bool Add(SetlistPosition element) => ManipulateData(element,(cntxt, setlistpos) =>
        {
            element.Setlist = cntxt.Setlists.Find(element.Setlist.Id);
            element.Song = cntxt.Songs.Find(element.Song.Id);
            cntxt.SetlistPositions.Add(setlistpos);
        });

        /// <see cref="IDataService{T}.Remove"/>
        public override bool Remove(SetlistPosition element) => ManipulateData(element, (cntxt, setlistpos) =>
        {
            cntxt.SetlistPositions.Remove(element);
            cntxt.SaveChanges();
            _positionUpdater.ReassignPositions(cntxt, _setlist);
        });

        /// <see cref="IDataService{T}.GetAll"/>
        public override List<SetlistPosition> GetAll()
        {
            try
            {
                using var dbCntxt = new DjSetsSqliteDbContext();
                var allSetlistPositions = dbCntxt
                    .SetlistPositions
                    .Include(it => it.Song)
                    .Where(it => it.Setlist == _setlist)
                    .OrderBy(it => it.Position)
                    .ToList();
                return allSetlistPositions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <see cref="IDataService{T}.Update"/>
        public override bool Update(SetlistPosition element) => ManipulateData(element, (cntxt, setlistpos) =>
        {
            cntxt.SetlistPositions.Update(setlistpos);
            cntxt.Setlists.Update(_setlist);
        });

        #endregion

        #region Functions from ExtendedOperationEfSqliteDataService
        /// <see cref="IExtendedModelOperationsDataService{T}.NumberOfElements"/>
        public override int NumberOfElements()
        {
            try
            {
                using var dbCntxt = new DjSetsSqliteDbContext();
                return dbCntxt
                    .SetlistPositions
                    .Count(it => it.Setlist == _setlist);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return 0;
            }
        }

        /// <see cref="IExtendedModelOperationsDataService{T}.AddAll"/>
        public override bool AddAll(List<SetlistPosition> elements)
        {
            return ManipulateData(elements, (context, positions) =>
            {
                context.SetlistPositions.AddRange(positions.Select(it =>
                {
                    it.Setlist = context.Setlists.Find(it.Setlist.Id);
                    it.Song = context.Songs.Find(it.Song.Id);
                    return it;
                }).ToList());
            });
        }

        /// <see cref="IExtendedModelOperationsDataService{T}.Clear"/>
        /// <remarks>Deletes all SetlistPositions of a Setlist</remarks>
        public override bool Clear()
        {
            try
            {
                using var context = new DjSetsSqliteDbContext();
                var remPos = context
                    .SetlistPositions
                    .Include(it => it.Setlist)
                    .Where(it => it.Setlist.Id == _setlist.Id)
                    .ToList();
                context.SetlistPositions.RemoveRange(remPos);
                bool delSuc = context.SaveChanges() > 0;

                if (delSuc)
                {
                    _positionUpdater.ReassignPositions(context, _setlist);
                }
                NotificationCenter.NotifyObservers();
                return delSuc;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        #endregion

        #region Functions for ISetlistPositionMover
        /// <see cref="ISetlistPositionMover.MovePosition"/>
        public bool MovePosition(SetlistPosition setlistPosition, int targetPosition)
        {
            try
            {
                var context = new DjSetsSqliteDbContext();
                return _positionUpdater.MoveToPosition(context, targetPosition, setlistPosition, _setlist);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        #endregion



    }
}
