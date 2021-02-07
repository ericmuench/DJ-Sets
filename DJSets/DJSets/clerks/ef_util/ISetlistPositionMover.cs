using DJSets.model.entityframework;

namespace DJSets.clerks.ef_util
{
    /// <summary>
    /// THis Interface defines logic for moving SetlistPositions of a certain Setlist
    /// </summary>
    interface ISetlistPositionMover
    {
        #region Functions
        /// <summary>
        /// This function defines a move-action of a SetlistPosition
        /// </summary>
        /// <param name="setlistPosition">The SetlistPosition to be moved</param>
        /// <param name="targetPosition">
        /// The targetPosition where <see cref="setlistPosition"/> should be moved to
        /// </param>
        /// <returns>Whether the Move-action was successful or not</returns>
        public bool MovePosition(SetlistPosition setlistPosition, int targetPosition);
        #endregion
    }
}
