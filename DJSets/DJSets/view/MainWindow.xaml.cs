using System.Windows;
using AdonisUI.Controls;
using DJSets.viewmodel.main;

namespace DJSets.view
{
    /// <summary>
    /// This class defines the logic of the MainWindow of this application.
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        #region Constructors
        public MainWindow()
        {
            var vm = new MainViewModel();
            vm.Init();
            DataContext = vm;
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        #endregion

    }
}
