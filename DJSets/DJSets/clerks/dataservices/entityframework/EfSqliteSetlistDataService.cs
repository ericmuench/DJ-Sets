using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DJSets.model.entityframework;
using DJSets.model.model_observe;
using Microsoft.EntityFrameworkCore;

namespace DJSets.clerks.dataservices.entityframework
{
    public class EfSqliteSetlistDataService : EfSqliteDataService<Setlist>
    {
        #region Constructors
        public EfSqliteSetlistDataService() : base(ModelNotificationCenter.SetlistNotifier){}
        #endregion

        #region Interface Functions for IDataService
        public override bool Add(Setlist element) => ManipulateData(element, (cntxt, setlist) =>
        {
            element.SetlistPositions = element.SetlistPositions?.Select(it =>
            {
                it.Song = cntxt.Songs.Find(it.Song.Id);
                return it;
            }).ToList();
            cntxt.Setlists.Add(setlist);
        });

        public override bool Remove(Setlist element) => ManipulateData(
            element, (cntxt, setlist) =>
            {
                cntxt.Setlists.Remove(cntxt.Setlists.Find(element.Id));
            });

        public override List<Setlist> GetAll()
        {
            try
            {
                using var dbCntxt = new DjSetsSqliteDbContext();
                return dbCntxt
                    .Setlists
                    .Include(it => it.SetlistPositions)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public override bool Update(Setlist element) => ManipulateData(element, (cntxt, setlist) =>
        {
            //cntxt.Setlists.Find(element.Id)
            cntxt.Setlists.Update(element);
        });

        #endregion


    }
}
