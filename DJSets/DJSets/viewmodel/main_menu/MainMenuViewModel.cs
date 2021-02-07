using System;
using DJSets.util.Extensions;
using DJSets.viewmodel.basics;

namespace DJSets.viewmodel.main_menu
{
    /// <summary>
    /// This class is the viewmodel for the MainMenu managing the state of the main menu and the viewmodels contained in the different sections
    /// </summary>
    class MainMenuViewModel : BaseViewModel
    {
        #region Fields
        /// <summary>
        /// This field manages the state of the Main menu
        /// </summary>
        private MainMenuState _mainMenuState;
        
        /// <summary>
        /// This field manages the value of <see cref="_mainMenuState"/> and calls the corresponding setters of <see cref="BaseViewModel"/>
        /// with the <see cref="OnMainMenuStateChanged"/> callback
        /// </summary>
        public MainMenuState MainMenuState
        {
            get => _mainMenuState;
            set => Set(ref _mainMenuState, value, nameof(MainMenuState),OnMainMenuStateChanged);
        }

        /// <summary>
        /// This field defines a callback that can be set in the UI to react to VM Data changes without a Binding but with a function of code
        /// </summary>
        public Action<MainMenuState> OnMainMenuStateChanged = (st) => { };
        #endregion

        #region Constructors
        /// <summary>
        /// This constructor creates a new MainViewmodel
        /// </summary>
        /// <param name="initState"> The state where the Main Menu should be in by default</param>
        /// <param name="onMainMenuStateChanged">Predefined action what will happen when State of the MainMenu changes</param>
        public MainMenuViewModel(MainMenuState initState, Action<MainMenuState> onMainMenuStateChanged = null)
        {
            onMainMenuStateChanged.NotNull(it => this.OnMainMenuStateChanged = it);
            MainMenuState = initState;
        }
        #endregion

    }
}
