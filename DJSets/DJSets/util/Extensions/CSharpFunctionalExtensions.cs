using System;

namespace DJSets.util.Extensions
{
    ///<summary>
    ///This class provides functional extensions for the C# Language
    /// </summary>
    public static class CSharpFunctionalExtensions
    {
        ///<summary>
        ///This function handles casting in a functional way, which means that
        /// code can be executed on a given object casted to a specific type.
        /// For this, Lambdas or delegates can be used.
        /// </summary>
        /// <example>
        ///     original way of casting
        ///     <code>
        ///         var person = new Person();
        ///         var st = (Student)person; // danger of running into an error if cast fails
        ///         //execute code with st 
        ///     </code>
        ///     functional way
        ///     <code>
        ///         var person = new Person();
        ///         person.CastedAs&lt;Student&gt;((st) => {
        ///             //do code with st 
        ///         });
        ///     </code>
        /// </example>
        /// <param name="original">the context object</param>
        /// <param name="action">Action to be executed with the context object casted as an object of type T</param>
        public static void CastedAs<T>(this object original, Action<T> action)
        {
            if(original == null) return;

            if (original is T cst)
            {
                action.Invoke(cst);
            }
        }


        ///<summary>
        ///This function handles null checks in a functional way, which means that
        /// code can be executed on a given object if it is not null.
        /// For this, Lambdas or delegates can be used.
        /// </summary>
        /// <example>
        ///     original way of null checks
        ///     <code>
        ///         var x = FunctionCallThatCanReturnNull();
        ///         if(x != null)
        ///         {
        ///             //execute code with x
        ///         }
        ///     </code>
        ///     functional way
        ///     <code>
        ///         var x = FunctionCallThatCanReturnNull();
        ///         x.NotNull((it) => {
        ///             //do code with it
        ///         });
        ///     </code>
        /// </example>
        /// <param name="source">the context object</param>
        /// <param name="action">Action to be executed with the context object if it is not null</param>
        public static void NotNull<T>(this T source, Action<T> action)
        {
            if (source != null)
            {
                action.Invoke(source);
            }
        }

        ///<summary>
        /// This function does the same as <see cref="NotNull{T}(T,System.Action{T})"/> but for Nullable-Objects
        /// </summary>
        /// <see cref="NotNull{T}(T,System.Action{T})"/>
        public static void NotNull<T>(this T? nullableSource, Action<T> action) where T : struct
        {
            if (nullableSource != null)
            {
                action(nullableSource.Value);
            }
        }


        ///<summary>
        ///This function handles configuring an object in a functional way.
        /// For this, Lambdas or delegates can be used.
        /// </summary>
        /// <example>
        ///     original way of applying
        ///     <code>
        ///         var p = new Person();
        ///         person.Name = "Max";
        ///         person.Age = 18;
        ///     </code>
        ///     functional way
        ///     <code>
        ///         var p = new Person().Apply((it) =>
        ///         {
        ///             it.Name = "Max";
        ///             it.Age = 18;
        ///         });
        ///     </code>
        /// </example>
        /// <param name="element">the context object</param>
        /// <param name="action">Action to be executed with the context object to configure it</param>
        /// <returns>The context object</returns>
        public static T Apply<T>(this T element, Action<T> action)
        {
            action(element);
            return element;
        }

        /// <summary>
        /// This generic function returns the given value if a certain condition is true. Else it will return a
        /// default value.
        /// </summary>
        /// <example>
        ///     In the following code-example the "GetString()"-Method returns a String.
        ///     The String should only be used when its length is smaller than 10.
        ///     <code>
        ///         var stringVal = GetString().TakeIf(str => str.Length &lt; 10);
        ///         /*stringVal will now be a String with a length smaller that 10 characters
        ///           or an empty String */
        ///     </code>
        /// </example>
        /// <typeparam name="T">The generic Type used for this function</typeparam>
        /// <param name="element">The element to be returned if the condition  in <see cref="condition"/> matches</param>
        /// <param name="condition">The condition that defines when to return the value or the alternative default value</param>
        /// <param name="defaultVal">The alternative default value to be returned if <see cref="condition"/> fails</param>
        /// <returns><see cref="element"/> if <see cref="condition"/> matches, else <see cref="defaultVal"/></returns>
        public static T TakeIf<T>(this T element, Func<T, bool> condition, T defaultVal = default(T)) 
        {
            if (condition(element))
            {
                return element;
            }

            return defaultVal;
        }
    }
}
