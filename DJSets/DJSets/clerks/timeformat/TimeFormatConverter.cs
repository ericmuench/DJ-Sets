using System;
using System.Text.RegularExpressions;
using DJSets.util.mvvm.validation;

namespace DJSets.clerks.timeformat
{
    /// <summary>
    /// This class defines a converter that can convert time strings to milliseconds and vice versa.
    /// </summary>
    public class TimeFormatConverter : IValidator<string>
    {
        #region Fields
        /// <summary>
        /// This field defines a Regex-Format for Validating Length-Input in the App
        /// </summary>
        private readonly Regex _minutesRegex = new Regex("^([1-9][0-9]*|0[0-9]?):[0-5][0-9]$");

        #endregion

        #region Functions
        /// <summary>
        /// This function takes a time string conforming to the defined regex patterns and converts it into the correct value in milliseconds
        /// </summary>
        /// <param name="timeString">The time string</param>
        /// <returns>DurationString in Milliseconds</returns>
        public long FormatToTimeMillis(string timeString)
        {
            if (_minutesRegex.IsMatch(timeString))
            {
                return GetMillisFromMinutesAndSeconds(timeString);
            }

            return 0;
        }

        /// <summary>
        /// This function formats the given time millis into a TimeFormat with the Pattern: MM:SS
        /// </summary>
        /// <param name="timeMillis">the time value to be converted</param>
        /// <returns>TimeFormat with the Pattern: MM:SS</returns>
        public string FormatToTimeString(long timeMillis)
        {
            long allInSeconds = ConvertToTimeSeconds(timeMillis);
            double minutes = allInSeconds / 60.0;
            long absoluteMinutes = (long) minutes;
            long remainingSeconds = (long) Math.Round((minutes - absoluteMinutes) * 60);
            return $"{FillUpWithZero(absoluteMinutes)}:{FillUpWithZero(remainingSeconds)}";
        }

        /// <summary>
        /// This function converts a ms- duration value into seconds
        /// </summary>
        /// <param name="timeMillis">The milliseconds duration value</param>
        /// <returns><see cref="timeMillis"/> in seconds</returns>
        public long ConvertToTimeSeconds(long timeMillis) => timeMillis / 1000;

        #endregion

        #region Initerface Functions for IValidator
        /// <summary>
        /// This function uses the the given Regex-Pattern to indicate whether a certain TimeString is valid and therefore can be converted.
        /// </summary>
        /// <param name="timeStr">TimeString from UI</param>
        /// <returns>Return whether <see cref="timeStr"/> conforms to the given Pattern to be converted into a valid time value in ms.</returns>
        public bool IsValid(string timeStr) =>
            _minutesRegex.IsMatch(timeStr);//_minutesTimeFormatRegex.IsMatch(timeStr)|| _hourTimeFormatRegex.IsMatch(timeStr) ;
        #endregion

        #region help functions
        /// <summary>
        /// This function returns the millis of a given time format
        /// </summary>
        /// <remarks>IMPORTANT: There should only be minutes and seconds specified in the string, which means only onw : in the String</remarks>
        /// <param name="timeStr">Time format string</param>
        /// <returns>time millis according to String</returns>
        /// 
        private long GetMillisFromMinutesAndSeconds(string timeStr)
        {
            var timeComponents = timeStr.Split(":");
            var minutes = long.Parse(timeComponents[^2]); // Length - 2
            var seconds = long.Parse(timeComponents[^1]); // Length - 1
            return (minutes * 60000) + (seconds * 1000);
        }

        /// <summary>
        /// This function formats the time value by giving him a 0-padding if its numeric value is smaller than 10
        /// </summary>
        /// <param name="num">time value</param>
        /// <returns>The time value as a string with a padding of zeros if it is smaller than 10</returns>
        private string FillUpWithZero(long num) => (num < 10) ? $"0{num}" : num.ToString();
        #endregion

        
    }
}
