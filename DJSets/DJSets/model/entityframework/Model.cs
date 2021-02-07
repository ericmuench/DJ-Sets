using System.Collections.Generic;
using DJSets.model.musickeys;

namespace DJSets.model.entityframework
{
    /// <summary>
    /// This class defines a Song Entity
    /// </summary>
    public class Song
    {
        #region Fields
        /// <summary>
        /// This property stores the id of the song for Entity Framework
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This property stores the title of the song
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// This property stores the name of the artist of the song
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// This property stores the length of the of the song in Milliseconds
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// This property stores the BPM-value of the of the song (Tempo value)
        /// </summary>
        public int Bpm { get; set; }

        /// <summary>
        /// This property stores the Key-value of the of the song
        /// </summary>
        public MusicKeys MusicKey { get; set; }

        /// <summary>
        /// This property stores the Genre of the of the song
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// This property stores the filepath of the of the song to get access to the corresponding file in FileSystem
        /// </summary>
        public string FilePath { get; set; }
        #endregion

        #region Relation Fields
        /// <summary>
        /// This property stores the related SetlistPositions to "know" which Setlists contain this song
        /// at a certain index.
        /// </summary>
        public List<SetlistPosition> SetlistPositions { get; set; }
        #endregion
    }

    /// <summary>
    /// This class defines a Setlist Entity
    /// </summary>
    public class Setlist
    {
        #region Fields
        /// <summary>
        /// This property stores the id of the Setlist for Entity Framework
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This property stores the title of the Setlist
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// This property stores the description of the Setlist
        /// </summary>
        public string Description { get; set; }
        #endregion

        #region Relation Fields
        /// <summary>
        /// This property stores the related SetlistPositions to "know" which song this Setlist
        /// contains and on with index it is located
        /// </summary>
        public List<SetlistPosition> SetlistPositions { get; set; }
        #endregion
    }

    /// <summary>
    /// This class defines a SetlistPosition-Entity
    /// </summary>
    /// <remarks>This class is necessary to persist the position of a <see cref="Song"/> in a <see cref="Setlist"/></remarks>
    public class SetlistPosition
    {
        #region Fields
        /// <summary>
        /// This property stores the id of the SetlistPosition for Entity Framework
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This property stores the position of the SetlistPosition
        /// </summary>
        public int Position { get; set; }
        #endregion

        #region Relation Fields
        /// <summary>
        /// This property stores the relating song for this SetlistPosition
        /// </summary>
        public Song Song { get; set; }

        /// <summary>
        /// This property stores the related Setlist where this SetlistPosition is located in
        /// </summary>
        public Setlist Setlist { get; set; }
        #endregion
    }
}
