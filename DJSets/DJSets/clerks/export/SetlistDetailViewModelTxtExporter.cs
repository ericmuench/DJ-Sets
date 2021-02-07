using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJSets.viewmodel.setlist.setlist_detail;

namespace DJSets.clerks.export
{
    /// <summary>
    /// This class can export a Setlist into a Txt-file
    /// </summary>
    /// <remarks>
    /// The exported txt-file does not conform any standard and should only be used to share a setlist via Text with
    /// other persons.
    /// </remarks>
    public class SetlistDetailViewModelTxtExporter : FileExporter<SetlistDetailViewModel>
    {
        #region Constructors
        public SetlistDetailViewModelTxtExporter(string filePath) : base(filePath) { }
        #endregion

        #region Implemented Abstract Functions from Superclass
        /// <summary>
        /// This function provides a Setlist as a txt-file-content. Title and Description is displayed first.
        /// After that, the songs should be displayed and at the end there should be the Setlist-Duration displayed.
        /// </summary>
        /// <see cref="FileExporter{T}.ProvideFileContent"/>
        protected override string ProvideFileContent(SetlistDetailViewModel element)
        {
            var textualSongs = element
                                   .SetlistPositionVMs
                                   ?.Select(vm => $"{vm.Position}) {vm.SongArtist} - {vm.SongTitle} ({vm.SongTempo},{vm.SongMusicKey})")
                               ?? new List<string>();
            return $"{element.SetListTitle}\n\n{element.SetListDescription}\n\n" +
                                     $"{string.Join("\n", textualSongs)}\n\n{element.SetListDurationString}";
        }
        #endregion

        #region Functions
        /// <see cref="FileExporter{T}.GetEncoding"/>
        protected override Encoding GetEncoding() => Encoding.UTF8;
        #endregion
    }
}
