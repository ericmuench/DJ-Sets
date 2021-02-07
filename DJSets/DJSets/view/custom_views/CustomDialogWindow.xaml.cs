using System;
using System.Windows;
using AdonisUI.Controls;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;

namespace DJSets.view.custom_views
{
    /// <summary>
    /// Interaktionslogik für CustomDialogWindow.xaml
    /// </summary>
    public partial class CustomDialogWindow : AdonisWindow
    {
        #region Constructors
        public CustomDialogWindow(FrameworkElement content, string title = "",Action<AdonisWindow> windowConfig = null)
        {
            InitializeComponent();

            //Assign Params + Further Configuration
            Title = title;
            Content = (content is HostedPage p) ? p.Apply(it => it.Host = this) : content;
            windowConfig?.Invoke(this);
        }
        #endregion
    }
}
