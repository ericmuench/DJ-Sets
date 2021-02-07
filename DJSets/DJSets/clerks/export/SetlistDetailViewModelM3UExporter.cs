using System.Diagnostics;
using System.IO;
using System.Text;
using DJSets.clerks.timeformat;
using DJSets.util.Extensions;
using DJSets.viewmodel.setlist.setlist_detail;

namespace DJSets.clerks.export
{
    /// <summary>
    /// This class can export a Setlist into a M3U-file for applications that can import a playlist.
    /// </summary>
    /// <remarks>
    /// The exported m3u-file can be used by DJ-Software that is able to import playlists based onn an M3U File.
    ///
    /// For further info, see https://en.wikipedia.org/wiki/M3U
    /// </remarks>
    /// 
    public class SetlistDetailViewModelM3UExporter : FileExporter<SetlistDetailViewModel>
    {
        #region Constructors
        public SetlistDetailViewModelM3UExporter(string filePath) : base(filePath) { }
        #endregion

        #region Clerks
        /// <summary>
        /// This field allows to convert time values
        /// </summary>
        private readonly TimeFormatConverter _timeFormatConverter = new TimeFormatConverter();

        #endregion

        #region Implemented Abstract Functions from Superclass
        /// <summary>
        /// This function provides a Setlist as a txt-file-content. Title and Description is displayed first.
        /// After that, the songs should be displayed and at the end there should be the Setlist-Duration displayed.
        /// The exported txt-file does not conform any standard and should only be used to share a setlist via Text with
        /// other persons.
        /// </summary>
        /// <see cref="FileExporter{T}.ProvideFileContent"/>
        protected override string ProvideFileContent(SetlistDetailViewModel element)
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine("#EXTM3U");
            strBuild.AppendLine($"#PLAYLIST:{element.SetListTitle}");
            
            foreach (var vm in element.SetlistPositionVMs)
            {
                var songFilePath = vm.GetElement().Song.FilePath;
                Debug.WriteLine($"Checking FilePath: {songFilePath}");
                if (File.Exists(songFilePath))
                {
                    var songDurationSeconds = _timeFormatConverter.ConvertToTimeSeconds(vm.GetElement().Song.Duration);
                    strBuild.AppendLine($"#EXTINF:{songDurationSeconds},{vm.SongArtist} – {vm.SongTitle}");
                    strBuild.AppendLine(songFilePath);
                }
            }

            return strBuild.ToString().Apply(it => Debug.WriteLine(it));
        }
        #endregion

        #region Functions
        /// <see cref="FileExporter{T}.GetEncoding"/>
        protected override Encoding GetEncoding() => Encoding.Unicode;
        #endregion
    }
}
