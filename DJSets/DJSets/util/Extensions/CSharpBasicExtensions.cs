using System;
using System.Collections.Generic;
using System.Linq;

namespace DJSets.util.extensions
{
    /// <summary>
    /// This class provides Extension functions for basic classes of C# e.g. string
    /// </summary>
    public static class CSharpBasicExtensions
    {
        #region String Extensions
        /// <summary>
        /// This extension function provides alternative way to checking the length to check whether a string is empty.
        /// </summary>
        /// <param name="str">The string to be checked if its empty</param>
        /// <returns>Returns whether the string is empty or not.</returns>
        public static bool IsEmpty(this string str) => str.Length == 0;

        /// <summary>
        /// This extension function counts how often a certain string appears in another
        /// </summary>
        /// <param name="str">the String to be checked for the other string</param>
        /// <param name="appStr">the searched string</param>
        /// <returns>How many times does <see cref="appStr"/> appear in <see cref="str"/> </returns>
        public static int CountAppearanceOf(this string str, string appStr) => str.Split(appStr).Length - 1;
        #endregion

        #region UInt Extensions
        /// <summary>
        /// This extension function converts a uint into an int
        /// </summary>
        /// <param name="uNum">The uint to be converted</param>
        /// <returns>
        /// The value of <see cref="uNum"/> as int or default of int if <see cref="uNum"/> is
        /// greater than int.MaxValue
        /// </returns>
        public static int ToInt(this uint uNum) 
            => (uNum > int.MaxValue) ? default : (int)uNum;
        #endregion

        #region IEnumerable Extension
        /// <summary>
        /// This extension function randomizes a <see cref="enumerable"/>.
        /// </summary>
        /// <remarks>
        /// This code was adopted by Stackoverflow:
        /// https://stackoverflow.com/questions/5383498/shuffle-rearrange-randomly-a-liststring
        /// --> LOC do not count
        /// </remarks>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to be randomized</param>
        /// <returns>A randomized <see cref="IEnumerable{T}"/> based on <see cref="enumerable"/></returns>
        public static IEnumerable<T> Randomized<T>(this IEnumerable<T> enumerable)
        {
            var random = new Random();
            return enumerable.OrderBy(it => random.Next());
        }

        #endregion

    }
}
