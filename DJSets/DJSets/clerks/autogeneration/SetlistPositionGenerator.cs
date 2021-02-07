using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DJSets.clerks.dataservices;
using DJSets.model.autogeneration;
using DJSets.model.entityframework;
using DJSets.model.musickeys;
using DJSets.util.extensions;
using DJSets.util.Extensions;

namespace DJSets.clerks.autogeneration
{
    /// <summary>
    /// This class defines logic for finding correct songs for Autogeneration of a Setlist and putting them into SetlistPositions
    /// </summary>
    public class SetlistPositionGenerator
    {
        #region Constructors
        public SetlistPositionGenerator(IDataService<Song> songService)
        {
            _songDataService = songService;
        }
        #endregion

        #region Clerks
        /// <summary>
        /// This field allows access to the persistence-Layer for songs
        /// </summary>
        private readonly IDataService<Song> _songDataService;

        #endregion

        #region Fields

        /// <summary>
        /// This field describes how much a bpm value can differ (+ and -) to still be relatively constant 
        /// </summary>
        private const int TempoBpmDifference = 5;

        /// <summary>
        /// This field describes a length buffer of 3 minutes in milliseconds.
        /// A generated Setlist should be maximal 3 minutes longer than the defined length dueto the fact that an exact length match is
        /// nearly impossible.
        /// </summary>
        private const long MaxExtraLength = 180000;

        #endregion

        #region Functions
        /// <summary>
        /// This function generates setlistpositions according to <see cref="autogenerationArgs"/>
        /// </summary>
        /// <remarks>CAUTION: This function should be exected in a Background Thread</remarks>
        /// <param name="autogenerationArgs">Pararmeter for Autogeneration</param>
        /// <param name="backgroundWorker">BackgroundWorker where this fucction is executed in to report progress</param>
        /// <returns>A List of generated SetlistPositions</returns>
        public List<SetlistPosition> GenerateSetlistPositions(BackgroundWorker backgroundWorker,SetlistAutogeneration autogenerationArgs)
        {
            backgroundWorker.ReportProgress(0);
            
            //filtering
            long currentSetlistLength = 0;
            var matchingSongs = _songDataService
                .GetAll()
                .Randomized()
                .Apply(_ => backgroundWorker.ReportProgress(13))
                .AsParallel()
                .Where(song => HasTempoMatch(song, autogenerationArgs)
                               && HasKeyMatch(song, autogenerationArgs)
                               && HasGenreMatch(song, autogenerationArgs))
                .Apply(_ => backgroundWorker.ReportProgress(25))
                .AsSequential()
                .TakeWhile(song =>
                {
                    var buffer = (autogenerationArgs.DesiredLength <= MaxExtraLength) ?  0 : MaxExtraLength;
                    currentSetlistLength += song.Duration;
                    return currentSetlistLength <= autogenerationArgs.DesiredLength + buffer;
                });

            
            backgroundWorker.ReportProgress(50);

            //ordering + mapping to setlistpositions
            var setlistPositions =
                SortSongs(matchingSongs, autogenerationArgs)
                    .Apply(_ => backgroundWorker.ReportProgress(75))
                    .Select((song, idx) => new SetlistPosition() { Position = idx, Song = song })
                    .ToList();
            
            backgroundWorker.ReportProgress(100);
            return setlistPositions;
        }

        #endregion

        #region Help functions
        /// <summary>
        /// This function checks if <see cref="Song"/> has a tempo that matches the requirements in <see cref="autogenerationArgs"/>
        /// </summary>
        /// <param name="song">The song to be checked</param>
        /// <param name="autogenerationArgs">The requirements defined for the autogeneration</param>
        /// <returns>Whether the song meets the requirements or not</returns>
        private bool HasTempoMatch(Song song, SetlistAutogeneration autogenerationArgs)
        {
            if (song.Bpm == 0 && autogenerationArgs.UseSongsWith0BpmToo)
            {
                return true;
            }

            switch (autogenerationArgs.TempoGenerationMode)
            { 
                case MovementGenerationMode.RelativelyConstant:
                    var constLowerBound = autogenerationArgs.StartTempo - TempoBpmDifference;
                    var constUpperBound = autogenerationArgs.StartTempo + TempoBpmDifference;
                    return song.Bpm >= constLowerBound && song.Bpm <= constUpperBound;
                case MovementGenerationMode.Rising:
                    return song.Bpm >= autogenerationArgs.StartTempo;
                case MovementGenerationMode.Falling:
                    return song.Bpm <= autogenerationArgs.StartTempo;
                default:
                    return false;
            }
        }

        /// <summary>
        /// This function checks if the key of the song matches the requirements for the autogeneration
        /// </summary>
        /// <remarks>If the requirements do not specify a constant key, this function might always return true</remarks>
        /// <param name="song">The song to be checked</param>
        /// <param name="autogenerationArgs">The requirements defined for the autogeneration</param>
        /// <returns>Whether there is a Key Match for <see cref="song"/></returns>
        private bool HasKeyMatch(Song song, SetlistAutogeneration autogenerationArgs)
        {
            if ((song.MusicKey == MusicKeys.Unidentified || song.MusicKey == MusicKeys.Dynamic)
                && autogenerationArgs.UseSongsWithUnidentifiedMusicKeyToo)
            {
                return true;
            }

            if (autogenerationArgs.MusicKeyGenerationMode != MovementGenerationMode.RelativelyConstant)
            {
                return song.MusicKey != MusicKeys.Unidentified && song.MusicKey != MusicKeys.Dynamic;
            }
            
            var surroundingKeys = autogenerationArgs.StartMusicKey.SurroundingMusicKeys();
            return song.MusicKey == autogenerationArgs.StartMusicKey || surroundingKeys.Contains(song.MusicKey);

        }

        /// <summary>
        /// This function checks if the Genre of the song matches the requirements for the autogeneration
        /// </summary>
        /// <param name="song">The song to be checked</param>
        /// <param name="autogenerationArgs">The requirements defined for the autogeneration</param>
        /// <returns>Whether <see cref="song"/> has the required genre defined in <see cref="autogenerationArgs"/></returns>
        private bool HasGenreMatch(Song song, SetlistAutogeneration autogenerationArgs) => autogenerationArgs.GenreGenerationMode switch
        {
            GeneralSpecificGenerationMode.Specific => song.Genre.Trim() == autogenerationArgs.DesiredGenre.Trim(),
            _ => true
        };

        /// <summary>
        /// This function sorts songs according to the autogeneration-requirements
        /// </summary>
        /// <param name="songs">the songs to be sorted</param>
        /// <param name="autogenerationArgs">The Autogeneration-Requirements</param>
        /// <returns><see cref="songs"/> but sorted in a way that matches <see cref="autogenerationArgs"/></returns>
        private IEnumerable<Song> SortSongs(IEnumerable<Song> songs, SetlistAutogeneration autogenerationArgs)
        {
            IComparer<MusicKeys> musicKeyComparer = new MusicKeyComparer(
                autogenerationArgs.StartMusicKey, 
                autogenerationArgs.MusicKeyGenerationMode == MovementGenerationMode.Rising);

            if (autogenerationArgs.TempoGenerationMode == MovementGenerationMode.Rising)
            {
                var risingTempoSongs = songs.OrderBy(it => it.Bpm);
                return autogenerationArgs.MusicKeyGenerationMode switch
                {
                    MovementGenerationMode.Rising =>
                        risingTempoSongs.ThenBy(it => it.MusicKey, musicKeyComparer),
                    MovementGenerationMode.Falling =>
                        risingTempoSongs.ThenBy(it => it.MusicKey, musicKeyComparer),
                    _ => risingTempoSongs
                };
            }

            if (autogenerationArgs.TempoGenerationMode == MovementGenerationMode.Falling)
            {
                var fallingTempoSongs = songs.OrderByDescending(it => it.Bpm);
                return autogenerationArgs.MusicKeyGenerationMode switch
                {
                    MovementGenerationMode.Rising =>
                        fallingTempoSongs.ThenBy(it => it.MusicKey, musicKeyComparer),
                    MovementGenerationMode.Falling =>
                        fallingTempoSongs.ThenBy(it => it.MusicKey, musicKeyComparer),
                    _ => fallingTempoSongs
                };
            }

            return autogenerationArgs.MusicKeyGenerationMode switch
            {
                MovementGenerationMode.Rising => songs.OrderBy(it => it.MusicKey, musicKeyComparer),
                MovementGenerationMode.Falling => songs.OrderBy(it => it.MusicKey, musicKeyComparer),
                _ => songs
            };
        }
        #endregion
    }
}
