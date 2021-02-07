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
    /// This class defines logic for providing CRUD-Operations to Songs using EF and a SQLite-DB.
    /// </summary>
    public class EfSqliteSongDataService : ExtendedOperationEfSqliteDataService<Song>, IDataMerger<Song>
    {
        #region Constructors
        public EfSqliteSongDataService() : base(ModelNotificationCenter.SongNotifier) { }
        #endregion

        #region Clerks
        /// <summary>
        /// This clerk should do all necessary update operations on SetlistPositions if there are
        /// changes in Songs
        /// </summary>
        private readonly SetlistPositionPositionUpdater _setlistPositionPositionUpdater 
            = new SetlistPositionPositionUpdater();

        #endregion

        #region Interface Functions for IDataService
        /// <see cref="EfSqliteDataService{T}.Add(T)"/>
        public override bool Add(Song element)
        {
            return ManipulateData(element, (context, song) => 
                context.Songs.Add(song));
        }

        /// <see cref="EfSqliteDataService{T}.Remove(T)"/>
        public override bool Remove(Song element)
        {
            return ManipulateData(element, (context, song) =>
            {
                var updateSetlists = GetSetlistsOfSong(context,song);
                context.Songs.Remove(song);
                context.SaveChanges();
                _setlistPositionPositionUpdater.ReassignPositions(context, updateSetlists);
            });
        }

        /// <see cref="EfSqliteDataService{T}.GetAll"/>
        public override List<Song> GetAll()
        {
            try
            {
                using var dbCntxt = new DjSetsSqliteDbContext();
                return dbCntxt.Songs.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <see cref="EfSqliteDataService{T}.Update(T)"/>
        public override bool Update(Song element)
        {
            return ManipulateData(element, (context, song) =>
            {
                context.Songs.Update(song);
            });
        }
        #endregion

        #region Interface Functions for ExtendedOperationDataService
        /// <see cref="IExtendedModelOperationsDataService{T}.NumberOfElements"/>
        public override int NumberOfElements()
        {
            try
            {
                using var context = new DjSetsSqliteDbContext();
                return context.Songs.Count();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return 0;
            }
        }

        /// <see cref="IExtendedModelOperationsDataService{T}.AddAll"/>
        public override bool AddAll(List<Song> elements) => ManipulateData(elements, (context, songs) =>
        {
            context.AddRange(songs);
        });

        /// <see cref="IExtendedModelOperationsDataService{T}.Clear"/>
        public override bool Clear()
        {
            try
            {
                using var context = new DjSetsSqliteDbContext();
                var updateSetlists = context
                    .Setlists
                    .Include(it => it.SetlistPositions)
                    .Where(it => it.SetlistPositions.Count > 0)
                    .ToList(); // --> all setlists that have songs in it via Setlistpositions
                context.Songs.RemoveRange(context.Songs.ToList());
                var delSuc = context.SaveChanges() > 0;
                if (delSuc)
                {
                    _setlistPositionPositionUpdater.ReassignPositions(context, updateSetlists);
                    //TODO: Eventually use this to reset ID-Range : see : https://www.xspdf.com/resolution/56258336.html
                    //context.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Songs';");
                }
                NotificationCenter.NotifyObservers();
                return delSuc;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }


        #endregion

        #region Interface functions for IDataMerger
        /// <see cref="IDataMerger{T}.Merge"/>
        public bool Merge(List<Song> elements, bool shouldRemoveOldData = false)
        {
            try
            {
                //getting current data
                var currentData = GetAll();
                
                if (currentData == null)
                {
                    return false;
                }

                //doing actual merge
                using var dbContext = new DjSetsSqliteDbContext();

                foreach (var newSong in elements.AsParallel())
                {
                    var similarOldSong = currentData
                        .FirstOrDefault(it => it.Title == newSong.Title && it.Artist == newSong.Artist);

                    if (similarOldSong != null)
                    {
                        //--> similar song has been found -> update
                        similarOldSong.Bpm = newSong.Bpm;
                        similarOldSong.Duration = newSong.Duration;
                        similarOldSong.FilePath = newSong.FilePath;
                        similarOldSong.Genre = newSong.Genre;
                        //similarOldSong.MusicKey = newSong.MusicKey;  --> Eventually use this when there is an intelligent key determination algorithm
                        dbContext.Songs.Update(similarOldSong);
                    }
                    else
                    {
                        //--> no similar Song --> new song needs to be added
                        dbContext.Songs.Add(newSong);
                    }
                }

                //remove old songs if necessary
                if (shouldRemoveOldData)
                {
                    var oldSongsToBeDeleted = currentData
                        .AsParallel()
                        .Where(oldElement =>
                        {
                            return elements.FirstOrDefault(newElement =>
                                oldElement.Title == newElement.Title && oldElement.Artist == newElement.Artist) == null;
                        }).ToList();
                    dbContext.Songs.RemoveRange(oldSongsToBeDeleted);

                }
                //save changes
                var affectedRows = dbContext.SaveChanges();
                NotificationCenter.NotifyObservers();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        #endregion

        #region Help Functions
        /// <summary>
        /// This function handles all operation that should be done on SetlistPositions when
        /// a song is deleted. Those operations are:
        ///     - Delete All SetlistPositions that contain the song to be deleted
        ///     - Reassign Positions of all Setlists that contained the song using <see cref="_setlistPositionPositionUpdater"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="song"></param>
        private List<Setlist> GetSetlistsOfSong(DjSetsSqliteDbContext context, Song song)
        {
            //getting data structures for the operations
            return context
                .Songs
                .Include(it => it.SetlistPositions)
                .ThenInclude( it => it.Setlist)
                .Where(it => it.Id == song.Id)
                .SelectMany(it => it.SetlistPositions)
                .GroupBy(it => it.Setlist.Id)
                .Select(it => it.Key)
                .ToList()
                .Select(it => context.Setlists.Find(it))
                .ToList();
        }

        #endregion


        
    }
}
