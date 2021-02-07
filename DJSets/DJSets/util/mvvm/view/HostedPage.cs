using System.Windows;
using System.Windows.Controls;
using DJSets.util.Extensions;

namespace DJSets.util.mvvm.view
{
    /// <summary>
    /// This abstract class defines a Page that has access to the UI-Component where it is hosted in (Window).
    /// </summary>
    public abstract class HostedPage : Page
    {
        #region Constructors
        /// <summary>
        /// This constructor creates a Page with a Window reference
        /// </summary>
        /// <param name="host">The Window where the Page is hosted in</param>
        public HostedPage(Window host = null)
        {
            Host = host;
            Host.NotNull(it =>
            {
                it.Title = this.Title;
            });
        }

        public HostedPage() : this(null){}
        #endregion

        #region Fields
        /// <summary>
        /// This field stores a reference to the window where the page is hosted in
        /// </summary>
        public Window Host;
        #endregion
    }
}
