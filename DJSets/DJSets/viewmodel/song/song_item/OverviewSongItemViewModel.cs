using DJSets.model.entityframework;
using DJSets.util.mvvm;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.song.song_item
{
    /// <summary>
    /// This class defines Viewmodel-Logic for a Song-Item in Menu-Lists (Overview Perspectives)
    /// </summary>
    public class OverviewSongItemViewModel : BaseViewModel, IElementContainer<Song>
    {
        #region Constructors
        public OverviewSongItemViewModel(Song song)
        {
            _song = song;
        }

        #endregion
        #region Fields
        /// <summary>
        /// This field contains the song for which this ViewModel provides Information 
        /// </summary>
        private readonly Song _song;

        /// <summary>
        /// This field provides a title for a Song
        /// </summary>
        public string Title
        {
            get => _song.Title;
        }

        /// <summary>
        /// This field provides the songs artist
        /// </summary>
        public string Artist
        {
            get => _song.Artist;
        }
        #endregion

        #region Interface Functions
        /// <see cref="IElementContainer{T}.GetElement()"/>
        public Song GetElement() => _song;
        #endregion
    }
}
