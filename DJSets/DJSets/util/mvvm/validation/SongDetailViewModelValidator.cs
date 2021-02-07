using System.IO;
using DJSets.clerks.timeformat;
using DJSets.util.extensions;
using DJSets.viewmodel.song.song_detail;

namespace DJSets.util.mvvm.validation
{
    /// <summary>
    /// This class is responsible for validating all fields of the <see cref="SongDetailViewModel"/>
    /// </summary>
    public class SongDetailViewModelValidator : IValidator<SongDetailViewModel>
    {
        #region Fields
        /// <summary>
        /// This field provides a <see cref="TimeFormatConverter"/> to check if the duration is valid
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();
        
        /// <summary>
        /// This clerk is able to validate the BPM Value
        /// </summary>
        private readonly TempoValidator _tempoValidator = new TempoValidator();

        #endregion

        #region Functions
        /// <summary>
        /// This function checks whether the title value is valid
        /// </summary>
        /// <param name="title">The Song Title</param>
        /// <returns>if the title-value is valid</returns>
        public bool IsValidTitle(string title) => !string.IsNullOrWhiteSpace(title);

        /// <summary>
        /// This function checks whether the artist value is valid
        /// </summary>
        /// <param name="artist">The Artist Name</param>
        /// <returns>if the artist-value is valid</returns>
        public bool IsValidArtist(string artist) => !string.IsNullOrWhiteSpace(artist);


        /// <summary>
        /// This function checks whether the tempo value is valid
        /// </summary>
        /// <param name="tempo">The Songs Tempo</param>
        /// <returns>if the tempo-value is valid</returns>
        public bool IsValidTempo(string tempo) => _tempoValidator.IsValid(tempo);


        /// <summary>
        /// This function checks whether the duration value is valid
        /// </summary>
        /// <param name="durationStr">The Songs DurationString as a string</param>
        /// <returns>if the duration-value is valid</returns>
        public bool IsValidDuration(string durationStr) => _timeFormatConverter.IsValid(durationStr);

        /// <summary>
        /// This function validates the filepath for a song
        /// </summary>
        /// <param name="path">given path for a certain music file</param>
        /// <returns>Whether the file at the given path exists or the path is empty</returns>
        public bool IsValidFilePath(string path) => path.IsEmpty() || File.Exists(path);

        #endregion

        #region Interface Functions for IValidator
        /// <see cref="IValidator{T}.IsValid"/>
        public bool IsValid(SongDetailViewModel vm) 
            => IsValidTitle(vm.Title)
               && IsValidArtist(vm.Artist)
               && IsValidDuration(vm.Duration)
               && IsValidTempo(vm.Tempo)
               && IsValidFilePath(vm.FilePath);

        #endregion
    }
}
