using System.Linq;
using DJSets.clerks.dataservices;
using DJSets.model.entityframework;

namespace DJSets.clerks.ef_util
{
    /// <summary>
    /// This class calculates the duration of a Setlist
    /// </summary>
    public class SetlistDurationCalculator
    {
        #region Functions
        /// <summary>
        /// This function is able to calculates the complete duration of a Setlist
        /// </summary>
        /// <remarks>CAUTION: Use this function in asynchronous code only</remarks>
        /// <param name="positionDataService">The DataService that handles the Setlist.</param>
        /// <returns>The complete duration of a Setlist in ms</returns>
        public long CalculateSetlistDuration(IDataService<SetlistPosition> positionDataService) => positionDataService
            .GetAll()
            .Select(it => it.Song.Duration)
            .Sum();
        #endregion
    }
}
