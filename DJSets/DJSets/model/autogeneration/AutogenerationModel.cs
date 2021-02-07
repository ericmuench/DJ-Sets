﻿using DJSets.model.musickeys;

namespace DJSets.model.autogeneration
{
    /// <summary>
    /// This class serves as an Container vor all necessary Parameters for the Setlist-Autogeneration-Process
    /// </summary>
    public class SetlistAutogeneration
    {
        #region Constructors
        public SetlistAutogeneration(
            MovementGenerationMode tempoMode,
            int startTempo,
            bool useSongsWith0BpmToo,
            MovementGenerationMode musicKeyGenerationMode,
            MusicKeys startKey,
            bool useSongsWithUnidentifiedMusicKeyToo,
            GeneralSpecificGenerationMode genreGenerationMode,
            string desiredGenre,
            long desiredLength)
        {
            TempoGenerationMode = tempoMode;
            StartTempo = startTempo;
            UseSongsWith0BpmToo = useSongsWith0BpmToo;
            MusicKeyGenerationMode = musicKeyGenerationMode;
            StartMusicKey = startKey;
            UseSongsWithUnidentifiedMusicKeyToo = useSongsWithUnidentifiedMusicKeyToo;
            GenreGenerationMode = genreGenerationMode;
            DesiredGenre = desiredGenre;
            DesiredLength = desiredLength;

        }
        #endregion

        #region Fields

        #region Fields for Generation
        /// <summary>
        /// This field describes how the tempo should evolve over time in the generated Setlist
        /// </summary>
        public readonly MovementGenerationMode TempoGenerationMode;

        /// <summary>
        /// This field describes with which tempo the first song of Setlist should start
        /// </summary>
        public readonly int StartTempo;

        /// <summary>
        /// This field describes whether songs with 0 BPM should also be used for the Setlist-Autogeneration
        /// independent of <see cref="StartTempo"/>
        /// </summary>
        /// <remarks>
        /// This can be used if there are many songs with an unknown BPM value that has the default of 0
        /// </remarks>
        public readonly bool UseSongsWith0BpmToo;

        /// <summary>
        /// This field describes how the MusicKey should evolve over time in the generated Setlist
        /// </summary>
        public readonly MovementGenerationMode MusicKeyGenerationMode;

        /// <summary>
        /// This field describes with which MusicKey the generated Setlist should start
        /// </summary>
        public readonly MusicKeys StartMusicKey;

        /// <summary>
        /// This field indicates whether songs with a value of MusicKeys.Unidentified should be used independent
        /// of <see cref="StartMusicKey"/>.
        /// </summary>
        /// <remarks>This can be helpful if there are many songs with an unidentified Key</remarks>
        public readonly bool UseSongsWithUnidentifiedMusicKeyToo;

        /// <summary>
        /// This field determines whether all genres should be used or just the specific genre specified in <see cref="DesiredGenre"/>
        /// </summary>
        public readonly GeneralSpecificGenerationMode GenreGenerationMode;
        
        /// <summary>
        /// The Genre all songs should have in the autogenerated Setlist.
        /// </summary>
        /// <remarks>This field is only used if <see cref="GenreGenerationMode"/> has the value "Specific" </remarks>
        public readonly string DesiredGenre;

        /// <summary>
        /// This field describes how long the Set should approximately be
        /// </summary>
        public readonly long DesiredLength;
        #endregion

        #endregion
    }
}
