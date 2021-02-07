using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;

namespace DJSets.model.musickeys
{
    /// <summary>
    /// This enum represents all Music Keys in Major and Minor
    /// </summary>
    public enum MusicKeys
    {
        #region values
        CMajor, CMinor, 
        CsharpMajor, CsharpMinor,
        DMajor, DMinor,
        DsharpMajor, DsharpMinor,
        EMajor, EMinor, 
        FMajor, FMinor, 
        FsharpMajor, FsharpMinor, 
        GMajor, GMinor, 
        GsharpMajor, GsharpMinor, 
        AMajor, AMinor, 
        AsharpMajor, AsharpMinor, 
        BMajor, BMinor,
        Dynamic, Unidentified
        #endregion
    }

    /// <summary>
    /// This class defines helpful methods and extension functions for working with the MusicKeys-Enum
    /// </summary>
    public static class MusicKeysUtils
    {
        #region Fields
        /// <summary>
        /// This field contains all parallel key-relations
        /// </summary>
        private static readonly List<Tuple<MusicKeys,MusicKeys>> ParallelKeys = new List<Tuple<MusicKeys, MusicKeys>>().Apply(list =>
        {
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.CMajor, MusicKeys.AMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.DMajor, MusicKeys.BMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.EMajor, MusicKeys.CsharpMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.FMajor, MusicKeys.DMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.GMajor, MusicKeys.EMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.AMajor, MusicKeys.FsharpMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.BMajor, MusicKeys.GsharpMinor));

            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.CsharpMajor, MusicKeys.AsharpMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.DsharpMajor, MusicKeys.CMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.FsharpMajor, MusicKeys.DsharpMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.GsharpMajor, MusicKeys.FMinor));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.AsharpMajor, MusicKeys.GMinor));
            
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.Dynamic, MusicKeys.Dynamic));
            list.Add(new Tuple<MusicKeys, MusicKeys>(MusicKeys.Unidentified, MusicKeys.Unidentified));
        });

        #endregion

        #region Extensions
        /// <summary>
        /// This function returns a String-Representation of the Music Key like it is usual for Musicians due to
        /// the fact that the Constant Names of the Music-Keys-Enum are not quite easy to read and do not conform the standard
        /// key-notation in music
        /// </summary>
        /// <param name="key">Key for which the extension function is for to easily access title via Method-Syntax</param>
        /// <returns></returns>
        public static string GetTitle(this MusicKeys key)
        {
            if (key == MusicKeys.Unidentified)
            {
                return "-";
            }
            
            if (key == MusicKeys.Dynamic)
            {
                return Application.Current.MainWindow.GetResource(StringResourceKeys.StrDynamicMusicKey,
                    "?");
            }

            var keyStr = key.ToString().ToLower();
            var additionalSigns = "";
            if (keyStr.Contains("sharp"))
            {
                additionalSigns += "#";
            }


            if (key.IsMinor())
            {
                additionalSigns += "m";
            }

            return $"{keyStr[0].ToString().ToUpper()}{additionalSigns}";

        }

        /// <summary>
        /// This extension function checks if the given MusicKey is a Minor-Key
        /// </summary>
        /// <param name="musicKey">the MusicKey that should be checked</param>
        /// <returns>Whether <see cref="musicKey"/> is minor or not</returns>
        public static bool IsMinor(this MusicKeys musicKey)
        {
            var keyStr = musicKey.ToString().ToLower();
            return keyStr.Contains("minor");
        }

        /// <summary>
        /// This extension function checks if a MusicKey is a Major-Key
        /// </summary>
        /// <param name="musicKey">the MusicKey that should be checked</param>
        /// <returns>Whether <see cref="musicKey"/> is major or not</returns>
        public static bool IsMajor(this MusicKeys musicKey)
            => (musicKey != MusicKeys.Unidentified && musicKey != MusicKeys.Dynamic) && !IsMinor(musicKey);

        /// <summary>
        /// This extension function returns the ParallelKey of <see cref="key"/> if it major else itself
        /// </summary>
        /// <param name="key">The key to be converted as minor</param>
        /// <returns>This extension function returns the ParallelKey of <see cref="key"/> if it major else itself</returns>
        public static MusicKeys AsMinor(this MusicKeys key)
        {
            if (key == MusicKeys.Unidentified || key == MusicKeys.Dynamic || key.IsMinor())
            {
                return key;
            }

            return key.ParallelMusicKey();
        }

        /// <summary>
        /// This extension function returns the ParallelKey of <see cref="key"/> if it minor else itself
        /// </summary>
        /// <param name="key">The key to be converted as major</param>
        /// <returns>This extension function returns the ParallelKey of <see cref="key"/> if it minor else itself</returns>
        public static MusicKeys AsMajor(this MusicKeys key)
        {
            if (key == MusicKeys.Unidentified || key == MusicKeys.Dynamic || key.IsMajor())
            {
                return key;
            }

            return key.ParallelMusicKey();
        }

        /// <summary>
        /// This extension function returns the number of semitones to a tone that is in the scale of <see cref="referenceKey"/>,
        /// meaning that e.g. if <see cref="musicKey"/> is Major and <see cref="referenceKey"/> is Minor than
        /// the amount of semitones from <see cref="musicKey"/> to the Parallel-MusicKey of <see cref="referenceKey"/> is returned.
        /// </summary>
        /// <param name="musicKey">The music key</param>
        /// <param name="referenceKey">the reference key to count to (how? --> <see cref="shouldCountDownwards"/>)</param>
        /// <param name="shouldCountDownwards">
        /// If true, the function counts downwards, meaning to use previous Music Key until it reaches the referenceKey(or its Parallel-Key
        /// if necessary). Otherwise it will go upwards. In short: If <see cref="shouldCountDownwards"/> is true, then <see cref="referenceKey"/>
        /// is always lower than <see cref="musicKey"/>. Otherwise it is always higher.
        /// </param>
        /// <returns>Number of Semitones from <see cref="musicKey"/> to scaleKey of <see cref="referenceKey"/></returns>
        public static int CountSemitonesToScale(this MusicKeys musicKey, MusicKeys referenceKey, bool shouldCountDownwards)
        {
            if (musicKey == MusicKeys.Unidentified 
                || referenceKey == MusicKeys.Unidentified 
                || musicKey == MusicKeys.Dynamic
                || referenceKey == MusicKeys.Dynamic)
            {
                return 0;
            }

            var targetKey = (musicKey.IsMinor()) ? referenceKey.AsMinor() : referenceKey.AsMajor();

            var semiToneCount = 0;
            var currentKey = musicKey;
            while (currentKey != targetKey)
            {
                currentKey = shouldCountDownwards ? currentKey.PreviousMusicKey() : currentKey.NextMusicKey();
                semiToneCount++;
            }

            return semiToneCount;
        }

        /// <summary>
        /// This extension returns the parallel MusicKey to <see cref="key"/> according to <see cref="ParallelKeys"/>
        /// </summary>
        /// <param name="key">The MusicKey of which the parallel MusicKey should be returned from this function</param>
        /// <returns>The parallel MusicKey for <see cref="key"/></returns>
        public static MusicKeys ParallelMusicKey(this MusicKeys key)
        {
            var keyPair = 
                ParallelKeys
                .FirstOrDefault(pair => key == pair.Item1 || key == pair.Item2);

            if (keyPair == null)
            {
                return MusicKeysDefault();
            }

            return (key == keyPair.Item1) ? keyPair.Item2 : keyPair.Item1;
        }

        /// <summary>
        /// This extension returns the next MusicKey that is one semitone higher than <see cref="key"/>
        /// </summary>
        /// <param name="key">The key to get the next key of</param>
        /// <returns>The next MusicKey of <see cref="key"/></returns>
        public static MusicKeys NextMusicKey(this MusicKeys key)
        {
            return key switch
            {
                MusicKeys.CMajor => MusicKeys.CsharpMajor,
                MusicKeys.CsharpMajor => MusicKeys.DMajor,
                MusicKeys.DMajor => MusicKeys.DsharpMajor,
                MusicKeys.DsharpMajor => MusicKeys.EMajor,
                MusicKeys.EMajor => MusicKeys.FMajor,
                MusicKeys.FMajor => MusicKeys.FsharpMajor,
                MusicKeys.FsharpMajor => MusicKeys.GMajor,
                MusicKeys.GMajor => MusicKeys.GsharpMajor,
                MusicKeys.GsharpMajor => MusicKeys.AMajor,
                MusicKeys.AMajor => MusicKeys.AsharpMajor,
                MusicKeys.AsharpMajor => MusicKeys.BMajor,
                MusicKeys.BMajor => MusicKeys.CMajor,

                MusicKeys.AMinor => MusicKeys.AsharpMinor,
                MusicKeys.AsharpMinor => MusicKeys.BMinor,
                MusicKeys.BMinor => MusicKeys.CMinor,
                MusicKeys.CMinor => MusicKeys.CsharpMinor,
                MusicKeys.CsharpMinor => MusicKeys.DMinor,
                MusicKeys.DMinor => MusicKeys.DsharpMinor,
                MusicKeys.DsharpMinor => MusicKeys.EMinor,
                MusicKeys.EMinor => MusicKeys.FMinor,
                MusicKeys.FMinor => MusicKeys.FsharpMinor,
                MusicKeys.FsharpMinor => MusicKeys.GMinor,
                MusicKeys.GMinor => MusicKeys.GsharpMinor,
                MusicKeys.GsharpMinor => MusicKeys.AMinor,

                _ => key
            };
        }

        /// <summary>
        /// This extension returns the previous MusicKey that is one semitone higher than <see cref="key"/>
        /// </summary>
        /// <param name="key">The key to get the previous key of</param>
        /// <returns>The previous MusicKey of <see cref="key"/></returns>
        public static MusicKeys PreviousMusicKey(this MusicKeys key)
        {
            return key switch
            {
                MusicKeys.CMajor => MusicKeys.BMajor,
                MusicKeys.CsharpMajor => MusicKeys.CMajor,
                MusicKeys.DMajor => MusicKeys.CsharpMajor,
                MusicKeys.DsharpMajor => MusicKeys.DMajor,
                MusicKeys.EMajor => MusicKeys.DsharpMajor,
                MusicKeys.FMajor => MusicKeys.EMajor,
                MusicKeys.FsharpMajor => MusicKeys.FMajor,
                MusicKeys.GMajor => MusicKeys.FsharpMajor,
                MusicKeys.GsharpMajor => MusicKeys.GMajor,
                MusicKeys.AMajor => MusicKeys.GsharpMajor,
                MusicKeys.AsharpMajor => MusicKeys.AMajor,
                MusicKeys.BMajor => MusicKeys.AsharpMajor,

                MusicKeys.AMinor => MusicKeys.GsharpMinor,
                MusicKeys.AsharpMinor => MusicKeys.AMinor,
                MusicKeys.BMinor => MusicKeys.AsharpMinor,
                MusicKeys.CMinor => MusicKeys.BMinor,
                MusicKeys.CsharpMinor => MusicKeys.CMinor,
                MusicKeys.DMinor => MusicKeys.CsharpMinor,
                MusicKeys.DsharpMinor => MusicKeys.DMinor,
                MusicKeys.EMinor => MusicKeys.DsharpMinor,
                MusicKeys.FMinor => MusicKeys.EMinor,
                MusicKeys.FsharpMinor => MusicKeys.FMinor,
                MusicKeys.GMinor => MusicKeys.FsharpMinor,
                MusicKeys.GsharpMinor => MusicKeys.GMinor,

                _ => key
            };
        }

        /// <summary>
        /// This extension determines the surrounding MusicKeys for <see cref="key"/>
        /// (All Keys that are one semitone higher and 1 semitone lower and the ParallelMusicKeys).
        /// </summary>
        /// <param name="key">The key to get the surrounding MusicKeys from</param>
        /// <returns>A List of surrounding MusicKeys for <see cref="key"/> and the parallel music key</returns>
        public static List<MusicKeys> SurroundingMusicKeys(this MusicKeys key)
        {
            var next = key.NextMusicKey();
            var prev = key.PreviousMusicKey();

            if (key == MusicKeys.Unidentified || key == MusicKeys.Dynamic)
            {
                return AllValues();
            }

            return new List<MusicKeys>(new[]
            {
                next,
                next.ParallelMusicKey(),
                prev,
                prev.ParallelMusicKey(),
                key.ParallelMusicKey()
            });
        }

        #endregion

        #region Functions
        /// <summary>
        /// This function shall return all values of the MusicKeys-Enum in a typesafe way
        /// </summary>
        /// <returns>List of all Music-Keys</returns>
        public static List<MusicKeys> AllValues()
        {
            var list = new List<MusicKeys>();
            Enum.GetValues(typeof(MusicKeys)).CastedAs<MusicKeys[]>(it =>
            {
                list.AddRange(it);
            });
            return list;
        }

        /// <summary>
        /// This function can convert a String/ Music-Key-Title back into a Music-Key.
        /// </summary>
        /// <param name="keyStr">String containing the title of a certain MusicKey</param>
        /// <param name="defaultVal">Default-Value that should be returned if conversion failed(e.g. because string is not a valid Music-Key-Title)</param>
        /// <returns>A MusicKeys-Enum-Value representing a certain MusicKey</returns>
        public static MusicKeys GetFromString(string keyStr,MusicKeys defaultVal = MusicKeys.Unidentified)
        {
            if (keyStr == null)
            {
                return defaultVal;
            }
            var matching = AllValues().Where(it => it.GetTitle() == keyStr).ToList();
            return (matching.Count() == 1) ? matching[0] : defaultVal;
        }

        /// <summary>
        /// This function provides a default value for the MusicKeys
        /// </summary>
        /// <returns>The Default Value for the music keys</returns>
        public static MusicKeys MusicKeysDefault() => MusicKeys.Unidentified;
        #endregion
    }

    /// <summary>
    /// This class defines a custom compare logic for <see cref="MusicKeys"/>
    /// </summary>
    public class MusicKeyComparer : IComparer<MusicKeys>
    {

        #region Constructors
        public MusicKeyComparer(MusicKeys referenceKey, bool shouldGoDownwards)
        {
            _referenceKey = referenceKey;
            _shouldGoDownwards = shouldGoDownwards;
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines a reference key. This key should be the "lowest key"
        /// </summary>
        private readonly MusicKeys _referenceKey;

        /// <summary>
        /// This field indicates whether <see cref="_referenceKey"/> is the next lower key (value is true) or the next higher (value is false)
        /// </summary>
        private readonly bool _shouldGoDownwards;

        #endregion

        #region Interface Functions for IComparer

        /// <see cref="IComparer{T}.Compare"/>
        /// <remarks>
        /// <see cref="mk2"/> will always be greater than <see cref="mk1"/> due to the fact that tones repeat
        /// in a scale
        /// </remarks>
        public int Compare(MusicKeys mk1, MusicKeys mk2)
        {
            Debug.WriteLine($"Comparing {mk1.GetTitle()} with {mk2.GetTitle()}");
            if (_referenceKey == MusicKeys.Unidentified || _referenceKey == MusicKeys.Dynamic)
            {
                return 0;
            }

            if (mk1 == mk2 || mk1 == mk2.ParallelMusicKey())
            {
                return 0;
            }

            if (mk1 == MusicKeys.Unidentified 
                || mk2 == MusicKeys.Unidentified 
                || mk1 == MusicKeys.Dynamic
                || mk2 == MusicKeys.Dynamic)
            {
                return 0;
            }

            var semiToneCount1 = mk1.CountSemitonesToScale(_referenceKey,_shouldGoDownwards);
            var semiToneCount2 = mk2.CountSemitonesToScale(_referenceKey,_shouldGoDownwards);

            return semiToneCount1 - semiToneCount2;
        }

        #endregion
    }
}
