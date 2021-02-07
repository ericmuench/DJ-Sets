using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using DJSets.model.entityframework;
using DJSets.model.musickeys;
using DJSets.resources;
using DJSets.util.extensions;
using DJSets.util.Extensions;

namespace DJSets.clerks.filesystem
{
    /// <summary>
    /// This class defines a Scanner that gets Song-Object out of Music-Files
    /// </summary>
    public class MusicFileScanner
    {
        #region Fields
        /// <summary>
        /// THis field defines all relevant music file extensions
        /// </summary>
        private readonly string[] _fileExtensions = { ".mp3", ".wav", ".ogg", ".flac", ".aac", ".wma" };

        #endregion

        #region Functions
        /// <summary>
        /// This function starts the file scan for music files
        /// </summary>
        /// <param name="worker">
        /// The worker that is used to scan the filesystem in background. This field is necessary to
        /// report progress to upper ArchitectureLayers like the UI.
        /// </param>
        /// <param name="rootDir">The root directory where the scan should start.</param>
        /// <returns>All Scanned Songs from the Filesystem.</returns>
        public List<Song> StartScan(BackgroundWorker worker, string rootDir)
        {
            worker.ReportProgress(0);
            var songs = AnalyzeDirectory(rootDir, rootDir, worker);
            worker.ReportProgress(100);
            return songs;

        }
        #endregion

        #region Functions for Scan-Algorithm
        /// <summary>
        /// This function gets a List of songs out of all files from a directory
        /// </summary>
        /// <param name="directory">The directory to get all songs from</param>
        /// <returns>A List of all Songs, created from all files of <see cref="directory"/> </returns>
        private List<Song> GetAllFiles(string directory)
        {
            return Directory
                .GetFiles(directory)
                .AsParallel()
                .Where(filePath => 
                    filePath.Contains(".") && _fileExtensions.Contains(
                        filePath.Substring(filePath.LastIndexOf(".", StringComparison.Ordinal))
                    )
                 )
                .Select(filePath =>
                {
                    var file = TagLib.File.Create(filePath);
                    var titleArtist = GetTitleAndArtist(file);
                    return new Song()
                    {
                        Title = titleArtist.Item1,
                        Artist = titleArtist.Item2,
                        Duration = (long)file.Properties.Duration.TotalMilliseconds,
                        Bpm = CalcBpm(file.Tag.BeatsPerMinute),
                        MusicKey = MusicKeys.Unidentified,
                        Genre = file.Tag.JoinedGenres,
                        FilePath = filePath
                    };
                })
                .ToList();
        }

        /// <summary>
        /// This function analyzes all directories and gets its files from it
        /// </summary>
        /// <remarks>This function uses recursion to get its results</remarks>
        /// <param name="path">Path info about which directory the function is currently working on</param>
        /// <param name="rootDir">The Information about the name of the Root-Directory</param>
        /// <param name="worker">The worker used for this algorithm. This field is needed for reporting progress.</param>
        /// <returns>A List of Songs from all Subdirectories of <see cref="rootDir"/></returns>
        private List<Song> AnalyzeDirectory(string path, string rootDir, BackgroundWorker worker)
        {
            string[] subDirs = Directory.GetDirectories(path);

            if (subDirs.Length == 0)
            {
                return GetAllFiles(path);
            }

            var songs = new List<Song>();

            for (var i = 0;i < subDirs.Length;i++)
            {
                var dir = subDirs[i];
                var dirFiles = AnalyzeDirectory(dir,rootDir, worker);
                songs.AddRange(dirFiles);

                //report progress on root
                if (path == rootDir)
                {
                    var percentage = (((double) i) / subDirs.Length) * 100;
                    worker.ReportProgress((int) percentage);
                }
            }

            var otherFilesInDirectory = GetAllFiles(path);
            songs.AddRange(otherFilesInDirectory);
            return songs;
        }
        #endregion


        #region Help Functions
        /// <summary>
        /// This function calculates the correct BPM Value for a Song due to the fact that TagLib
        /// Library sometimes determines BPM not correctly but just with too much zeros in the value.
        /// </summary>
        /// <param name="uBpm">The BPM Value from TagLib</param>
        /// <returns>A BPM Value that might make more sense than the value from TagLib</returns>
        private int CalcBpm(uint uBpm)
        {
            var iBpm = uBpm.ToInt();

            if (iBpm > 1000)
            {
                return int.Parse(iBpm.ToString().Substring(0, 3));
            }

            return iBpm;
        }

        /// <summary>
        /// This function tries to determine the correct title and artist value for a song based on
        /// TTagLib and/or the Filename 
        /// </summary>
        /// <param name="file">The file to get the Song-Title and Song-Artist from</param>
        /// <returns>A Tuple for Title and Artist (First is Title, Second is artist)</returns>
        private Tuple<string, string> GetTitleAndArtist(TagLib.File file)
        {
            var title = file.Tag.Title?.Trim() ?? string.Empty;
            var artist = file.Tag.JoinedPerformers?.Trim() ?? string.Empty;

            var titleAndArtistFromFileName = GetTitleAndArtistFromFileName(file);

            if (title.IsEmpty())
            {
                title = titleAndArtistFromFileName.Item1;
            }

            if (artist.IsEmpty())
            {
                artist = titleAndArtistFromFileName.Item2;
            }

            return new Tuple<string, string>(title, artist);

        }

        /// <summary>
        /// This function parses the title and artist information out of the songs filename
        /// </summary>
        /// <param name="file">The file where artist and title information should be parsed from</param>
        /// <returns>The title and artist info for the song based on the filename</returns>
        private Tuple<string,string> GetTitleAndArtistFromFileName(TagLib.File file)
        {
            var pureName = GetPureFileName(file).Trim();
            if (!pureName.Contains("-"))
            {
                var defaultArtist = Application.Current.GetResource(
                        StringResourceKeys.StrUnknownArtist, "Unknown Artist"
                    );
                var defaultTitle = (!pureName.IsEmpty()) ? pureName : Application.Current.GetResource(
                    StringResourceKeys.StrUnknownTitle, "Unknown Title"
                );

                return new Tuple<string, string>(defaultTitle,defaultArtist);
            }

            var seperateIndex = pureName.IndexOf("-", StringComparison.Ordinal);
            var artist = pureName.Substring(0, seperateIndex).Trim();
            var title = (seperateIndex == pureName.Length - 1) ? artist : pureName.Substring(seperateIndex + 1).Trim();

            return new Tuple<string, string>(title,artist);
        }

        /// <summary>
        /// This function analyzes <see cref="file"/> and gets the pure name out of it
        /// </summary>
        /// <param name="file">The File to be analyzed</param>
        /// <returns>The pure filename without directory paths or file extensions</returns>
        private string GetPureFileName(TagLib.File file)
        {
            var fileName = Path.GetFileName(file.Name)?.ToString() ?? string.Empty;
            if (!fileName.Contains("."))
            {
                return fileName;
            }

            var actualNameLength = fileName.LastIndexOf(".", StringComparison.Ordinal);
            return fileName.Substring(0, actualNameLength);
        }


        #endregion
    }
}
