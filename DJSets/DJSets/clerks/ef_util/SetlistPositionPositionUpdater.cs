using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DJSets.model.entityframework;
using Microsoft.EntityFrameworkCore;

namespace DJSets.clerks.ef_util
{
    /// <summary>
    /// This clerk handles Updates to SetlistPositions that have something to do with the
    /// Position-Value
    /// </summary>
    /// <remarks>This class should be used as a helper-clerk, e.g. for a DataService</remarks>
    public class SetlistPositionPositionUpdater
    {
        #region Functions
        /// <summary>
        /// This function reassigns all positions to avoid holes or double assigned positions
        /// </summary>
        /// <example>
        /// A Setlist has 5 Songs. If Song No. 3 is deleted, then 4 songs remain. Their position-values
        /// are 0,1,3 and 4. As a consequence, there is a "hole" in position values. Further to that, a newly
        /// added Setlist Position would get the position-value 4 due to the fact that new SetlistPositions are always added
        /// at the last position of the Setlist,meaning that the add-algorithm only cares about the absolute count
        /// of SetlistPositions in a Setlist but now about their position-values. This function will take care of this
        /// issue by looping over all SetlistPosition-Entries of a Setlist and assigning their positions to a new value.
        /// </example>
        /// <param name="dbContext">The DBContext to operate with</param>
        /// <param name="setlist">The Setlist where the SetlistPositions should be reassigned</param>
        /// <returns>Whether the Reassign-Operation was successful or not</returns>
        public bool ReassignPositions(DjSetsSqliteDbContext dbContext, Setlist setlist)
        {
            try
            {
                //Get all SetlistPositions of the given setlist ordered by their position
                var corPositions = dbContext
                    .SetlistPositions
                    .Where(pos => pos.Setlist.Id == setlist.Id)
                    .OrderBy(pos => pos.Position)
                    .ToList();

                //update position value
                for (int i = 0; i < corPositions.Count; i++)
                {
                    corPositions[i].Position = i;
                }

                //DB Update
                dbContext.SetlistPositions.UpdateRange(corPositions);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// This function does the same as the other ReassignPositions-Function
        /// but for multiple Setlists
        /// </summary>
        /// <param name="dbContext">The DBContext to operate with</param>
        /// <param name="setlists">The Setlists where a Position-Reassign Operation is necessary</param>
        /// <returns>Success for each Setlist-Reassign Operation, stored in a List of Booleans</returns>
        public List<bool> ReassignPositions(DjSetsSqliteDbContext dbContext, List<Setlist> setlists)
            => setlists.Select(setlist => ReassignPositions(dbContext, setlist)).ToList();


        /// <summary>
        /// This function allows moving a certain SetlistPosition to another Position by ajusting the
        /// Position value. Additionally to that all other Positions are rearranged to form a consistent
        /// order of SetlistPositions.
        /// </summary>
        /// <param name="dbContext">The Context to operate with for DB-Operations</param>
        /// <param name="targetPosition">the position where <see cref="movePosition"/> should be moved to</param>
        /// <param name="movePosition">The SetlistPosition that should be moved</param>
        /// <param name="setlist">The Setlist where the movement takes place</param>
        /// <returns>Whether the movement action was successful or not</returns>
        public bool MoveToPosition(DjSetsSqliteDbContext dbContext, 
            int targetPosition, 
            SetlistPosition movePosition,
            Setlist setlist)
        {
            try
            {
                var sourcePosition = movePosition.Position;
                int otherPositionMoveFactor;
                Func<SetlistPosition, bool> positionFilter; 
                if (sourcePosition < targetPosition)
                {
                    //moveposition needs to be moved downwards & all other positions should be moved upwards
                    otherPositionMoveFactor = -1; //Other Positions need to be moved upwards
                    positionFilter = pos => 
                            pos.Setlist.Id == setlist.Id &&
                            pos.Position <= targetPosition &&
                            pos.Position > sourcePosition;
                }
                else if(sourcePosition > targetPosition)
                {
                    //moveposition needs to be moved upwards & all other positions should be moved downwards
                    otherPositionMoveFactor = 1; //Other Positions need to be moved downwards
                    positionFilter = pos =>
                        pos.Setlist.Id == setlist.Id &&
                        pos.Position >= targetPosition &&
                        pos.Position < sourcePosition;
                }
                else
                {
                    //Source and target positions seem to be the same --> no update needed
                    return false;
                }

                //get all other positions that need to be moved
                var otherPositions = dbContext
                    .SetlistPositions
                    .Include(it => it.Setlist)
                    .Where(positionFilter)
                    .OrderBy(pos => pos.Position)
                    .ToList();

                //update otherPositions
                foreach (var otherPos in otherPositions)
                {
                    //all other positions need to be moved upwards --> index/position value get smaller
                    otherPos.Position += otherPositionMoveFactor;
                }

                dbContext.UpdateRange(otherPositions);
                dbContext.SaveChanges();

                //moving element
                //movePosition.Position = targetPosition;
                var dbMovePosition = dbContext.SetlistPositions.Find(movePosition.Id);
                dbMovePosition.Position = targetPosition;
                dbContext.Update(dbMovePosition);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        #endregion
    }
}
