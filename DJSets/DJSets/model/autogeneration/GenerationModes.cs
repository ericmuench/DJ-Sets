using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;

namespace DJSets.model.autogeneration
{
    #region Generation Modes
    /// <summary>
    /// This enum describes a GenerationMode, meaning how certain parameters need to be generated.
    /// This particular enum contains information about how a parameter should change as it "moves" resp.
    /// how it should change over time
    /// </summary>
    /// <remarks>
    /// There are 3 Modes:
    ///     - RelativelyConstant = The value should stay relatively constant as times changes
    ///     - Rising = The value should get higher as time changes
    ///     - Falling = The value should get lower as time changes
    /// </remarks>
    public enum MovementGenerationMode
    {
        RelativelyConstant, Rising, Falling
    }

    /// <summary>
    /// This enum describes a GenerationMode, meaning how certain parameters need to be generated.
    /// This particular enum contains information whether a value uses a specific value or all values
    /// </summary>
    /// <remarks>
    /// There are 2 Modes:
    ///     - General = Use all values
    ///     - Specific = Use the specific value
    /// </remarks>
    public enum GeneralSpecificGenerationMode
    {
        General, Specific
    }
    #endregion

    #region Generation Mode Utils
    /// <summary>
    /// This class defines helpful methods and extension functions for working with the MovementGenerationMode-Enum
    /// </summary>
    public static class MovementGenerationModeUtils
    {
        #region Extensions
        /// <summary>
        /// This function returns a string representation of <see cref="mode"/> accessible via extension function syntax
        /// </summary>
        /// <param name="mode">The mode to get the title from</param>
        /// <returns>The title for <see cref="mode"/></returns>
        public static string GetTitle(this MovementGenerationMode mode)
        {
            var app = Application.Current;
            switch (mode)
            {
                case MovementGenerationMode.RelativelyConstant: 
                    return app.GetResource<string>(StringResourceKeys.StrTitleMovementGenerationModeRelativelyConstant);
                case MovementGenerationMode.Rising:
                    return app.GetResource<string>(StringResourceKeys.StrTitleMovementGenerationModeRising);
                case MovementGenerationMode.Falling:
                    return app.GetResource<string>(StringResourceKeys.StrTitleMovementGenerationModeFalling);
                default: return string.Empty;
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// This function shall return all values of the MovementGenerationMode-Enum in a typesafe way
        /// </summary>
        /// <returns>List of all MovementGenerationModes</returns>
        public static List<MovementGenerationMode> AllValues()
        {
            var list = new List<MovementGenerationMode>();
            Enum.GetValues(typeof(MovementGenerationMode)).CastedAs<MovementGenerationMode[]>(it =>
            {
                list.AddRange(it);
            });
            return list;
        }

        /// <summary>
        /// This function can convert a String/ MovementGenerationMode-Title back into a MovementGenerationMode.
        /// </summary>
        /// <param name="keyStr">String containing the title of a certain MovementGenerationMode</param>
        /// <param name="defaultVal">Default-Value that should be returned if conversion failed(e.g. because string is not a valid MovementGenerationMode)</param>
        /// <returns>A MovementGenerationMode-Enum-Value representing a certain MovementGenerationMode</returns>
        public static MovementGenerationMode GetFromString(string keyStr, MovementGenerationMode defaultVal = MovementGenerationMode.RelativelyConstant)
        {
            if (keyStr == null)
            {
                return defaultVal;
            }
            var matching = AllValues().Where(it => it.GetTitle() == keyStr).ToList();
            return (matching.Count() == 1) ? matching[0] : defaultVal;
        }

        /// <summary>
        /// This function provides a default value for the MovementGenerationMode
        /// </summary>
        /// <returns>The Default Value for the MovementGenerationModes</returns>
        public static MovementGenerationMode MovementGenerationModesDefault() => MovementGenerationMode.RelativelyConstant;
        #endregion
    }

    /// <summary>
    /// This class defines helpful methods and extension functions for working with the MovementGenerationMode-Enum
    /// </summary>
    public static class GeneralSpecificGenerationModeUtils
    {
        #region Extensions
        /// <summary>
        /// This function returns a string representation of <see cref="mode"/> accessible via extension function syntax
        /// </summary>
        /// <param name="mode">The mode to get the title from</param>
        /// <returns>The title for <see cref="mode"/></returns>
        public static string GetTitle(this GeneralSpecificGenerationMode mode)
        {
            var app = Application.Current;
            switch (mode)
            {
                case GeneralSpecificGenerationMode.General:
                    return app.GetResource<string>(StringResourceKeys.StrTitleGeneralSpecificGenerationModeGeneral);
                case GeneralSpecificGenerationMode.Specific:
                    return app.GetResource<string>(StringResourceKeys.StrTitleGeneralSpecificGenerationModeSpecific);
                default: return string.Empty;
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// This function shall return all values of the GeneralSpecificGenerationMode-Enum in a typesafe way
        /// </summary>
        /// <returns>List of all GeneralSpecificGenerationModes</returns>
        public static List<GeneralSpecificGenerationMode> AllValues()
        {
            var list = new List<GeneralSpecificGenerationMode>();
            Enum.GetValues(typeof(GeneralSpecificGenerationMode)).CastedAs<GeneralSpecificGenerationMode[]>(it =>
            {
                list.AddRange(it);
            });
            return list;
        }

        /// <summary>
        /// This function can convert a String/ GeneralSpecificGenerationMode-Title back into a GeneralSpecificGenerationMode.
        /// </summary>
        /// <param name="keyStr">String containing the title of a certain GeneralSpecificGenerationMode</param>
        /// <param name="defaultVal">Default-Value that should be returned if conversion failed(e.g. because string is not a valid GeneralSpecificGenerationMode)</param>
        /// <returns>A GeneralSpecificGenerationMode-Enum-Value representing a certain GeneralSpecificGenerationMode</returns>
        public static GeneralSpecificGenerationMode GetFromString(string keyStr, GeneralSpecificGenerationMode defaultVal = GeneralSpecificGenerationMode.General)
        {
            if (keyStr == null)
            {
                return defaultVal;
            }
            var matching = AllValues().Where(it => it.GetTitle() == keyStr).ToList();
            return (matching.Count() == 1) ? matching[0] : defaultVal;
        }

        /// <summary>
        /// This function provides a default value for the GeneralSpecificGenerationMode
        /// </summary>
        /// <returns>The Default Value for the GeneralSpecificGenerationModes</returns>
        public static GeneralSpecificGenerationMode GeneralSpecificGenerationModesDefault() => GeneralSpecificGenerationMode.General;


        #endregion
    }
    #endregion

}
