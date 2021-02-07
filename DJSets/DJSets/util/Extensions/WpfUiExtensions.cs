using System.Windows;
using DJSets.resources;

namespace DJSets.util.Extensions
{
    ///<summary>
    ///This class provides ui extensions for wpf
    /// </summary>
    public static class WpfUiExtensions
    {
        /// <summary>
        /// This function allows to load resources in Code behind with generic type and handles possible type-cast exceptions via default value.
        /// </summary>
        /// <example>
        ///     How to normally return resources in code  
        ///     <code>
        ///         string stringRes = (string)this.FindResource("key"); //danger of exception if cast fails
        ///     </code>
        ///     How to get a resource with this functions
        ///     <code>
        ///         string stringRes = this.GetResource&lt;string&gt;("key","default value");
        ///     </code>
        /// </example>
        ///<param name="element">FrameworkElement that is able to load a resource (e.g. Window,UserControl,etc)</param>
        ///<param name="key">Key for the resource that should be loaded (originally defined in xaml-Resource-Dictionarys)</param>
        ///<param name="defaultVal">provided default value that is returned if the requested resource does not exist or is not of the given type T</param>
        ///<returns>The resource associated with the given key as an object of the given type T</returns> 
        /// 
        public static T GetResource<T>(this FrameworkElement element, string key,T defaultVal = default(T))
        {
            var searchRes = element.TryFindResource(key);
            if (searchRes == null)
            {
                return defaultVal;
            }

            if (searchRes is T convertedToType)
            {
                return convertedToType;
            }

            return defaultVal;
        }

        /// <summary>
        /// This function does the same as <see cref="GetResource{T}(System.Windows.FrameworkElement,string,T)"/> but on  the application and not on a specific FrameworkElement
        /// </summary>
        public static T GetResource<T>(this Application app, string key, T defaultVal = default(T))
        {
            var searchRes = app.TryFindResource(key);
            if (searchRes == null)
            {
                return defaultVal;
            }

            if (searchRes is T convertedToType)
            {
                return convertedToType;
            }

            return defaultVal;
        }


        /// <summary>
        /// This function allows to easily display a MessageBox with using resource-keys.
        /// </summary>
        /// <param name="element">this element that can use the <see cref="GetResource{T}(System.Windows.FrameworkElement,string,T)"/> extension function</param>
        /// <param name="titleKey">Key of the Title String contained in <see cref="StringResourceKeys"/></param>
        /// <param name="msgKey">Key of the message string, contained in <see cref="StringResourceKeys"/></param>
        /// <param name="btn">buttons of the message box</param>
        /// <param name="img">image of the message box</param>
        /// <returns>Result of the MessageBox</returns>
        public static MessageBoxResult ShowMessageBoxWithResources(
            this FrameworkElement element,
            string titleKey,
            string msgKey,
            MessageBoxButton btn = MessageBoxButton.OKCancel,
            MessageBoxImage img = MessageBoxImage.Warning)
        {
            var title = element.GetResource<string>(titleKey);
            var msg = element.GetResource<string>(msgKey);
            return ShowMessageBox(title, msg,btn,img);
        }

        /// <summary>
        /// This function displays a message box
        /// </summary>
        /// <param name="title">title of the message box</param>
        /// <param name="msg">message of the message box</param>
        /// <param name="btn">buttons of the message box</param>
        /// <param name="img">image of the message box</param>
        /// <returns>Result of the MessageBox</returns>
        public static MessageBoxResult ShowMessageBox(
            string title,
            string msg,
            MessageBoxButton btn = MessageBoxButton.OKCancel,
            MessageBoxImage img = MessageBoxImage.Warning) =>
            MessageBox.Show(msg, title, btn, img);
    }
}
