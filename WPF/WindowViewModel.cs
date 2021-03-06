﻿using System;
using System.Windows;
using System.Windows.Input;
using MES_2.Modules.ComModule;
using MES_2.Modules.PLCConnectorModule;
using MES_2.Modules.SystemModule.Entity;
using MES_2.Modules.SystemModule.State;
using MES_2.Modules.SystemModule.Translation;
using MES_2.Modules.UserManagement;
using WPF.Helpers;
using WPF.Modules.Base;
using WPF.Modules.ComModule;
using WPF.Modules.Historian;
using WPF.Modules.PLCConnectorModule;
using WPF.Modules.SystemModule.Authentication;
using WPF.Modules.SystemModule.Entity;
using WPF.Modules.SystemModule.Login;
using WPF.Modules.SystemModule.State;
using WPF.Modules.SystemModule.Translation;
using WPF.Modules.UserManagement;
using WPF.ViewModel;
using WPF.Window;

namespace WPF
{
    public class WindowViewModel : ViewModelBase
    {
        // ViewModels of Modules
        public AuthenticationViewModel AuthVM { get; set; }
//        public LoginControlViewModel LoginVM { get; set; }
        private string _CurrentPageName = "Login Control";
        public string CurrentPageName
        {
            get { return _CurrentPageName; }
            set{SetProperty(ref _CurrentPageName, value);}
        }

        private ViewModelBase _CurrentPage;
        public ViewModelBase CurrentPage
        {
            get { return _CurrentPage; }
            set
            { SetProperty(ref _CurrentPage, value);}
        }

        public RelayCommand<string> NavCommand { get; private set; }
        //Command list of window
        //konstruktor muze byt z nastaveni okna
        public WindowViewModel(System.Windows.Window p_window)
        {
            mWindow = p_window;
            AuthVM = AuthenticationViewModel.Instance; // autentizace take
            LoginControlViewModel Temp = new LoginControlViewModel();
            Temp.SuccesFullLogin += NavToProfileUser;
            CurrentPage = Temp;

            NavCommand = new RelayCommand<string>(OnNav);

            // Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                WindowResized();
            };

            // Create commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));

            // Fix window resize issue
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                mDockPosition = dock;

                // Fire off resize events
                WindowResized();
            };

        }

        #region Navigation

        private void OnNav(string p_destination)
        {
            switch (p_destination)
            {
                case "ProfileView":
                    NavToProfileUser();
                    break;
                case "CommucationView":
                    NavToCommucation();
                    break;
                case "Historian":
                    NavToHistorian();
                    break;
                case "UserManagementView":
                    NavToUserManagement();
                    break;
                case "EntitiesView":
                    NavToEntity();
                    break;
                case "StatesEntitiesView":
                    NavToState(0);
                    break;
                case "TranslationView":
                    NavToTranslation(0);
                    break;
                case "PLCConnectorView":
                    NavToPLCConnectorModule();
                    break;

                default:
                    break;
            }

        }
        private void NavToUserManagement()
        {
            UserManagementViewModel Temp = new UserManagementViewModel();
            Temp.AddEditUserRequested += NavToAddEditUser;
            CurrentPage = Temp;
            CurrentPageName = "User Management";
        }

        private void NavToProfileUser()
        {
            // jeste zjistit jestli na to ma uzivatel prava
            ProfileUserViewModel Temp = new ProfileUserViewModel();
            //Temp.EditViewRequest+= 
            //Temp.StateViewRequested += NavToStateEntiti;
            Temp.AddEditUserRequested += NavToAddEditUser;
            CurrentPage = Temp;
            CurrentPageName = "Profile";
            CurrentPage.IsRightsMode = true; //volani permission modulu

        }
        private void NavToAddEditUser(UserFull p_user)
        {
            // jeste zjistit jestli na to ma uzivatel prava
            AddEditUserManagementViewModel Temp = new AddEditUserManagementViewModel(p_user);

            Temp.Done += NavToProfileUser;
            CurrentPage = Temp;
            CurrentPageName = "Add/Edit Page";
            CurrentPage.IsRightsMode = true; //volani permission modulu
        }

        private void NavToCommucation()
        {
            // jeste zjistit jestli na to ma uzivatel prava
            CurrentPage = new CommunicationPageViewModel();
            CurrentPageName = "Communication Manager";
        }
        private void NavToHistorian()
        {
            // jeste zjistit jestli na to ma uzivatel prava
            CurrentPage = new HistorianPageViewModel();
            CurrentPageName = "Historian";
        }

        private void NavToPLCConnectorModule()
        {
            // jeste zjistit jestli na to ma uzivatel prava
            PLCConnectorViewModel Temp = new PLCConnectorViewModel();
            Temp.AddEditPLCRequested += NavToAddEditPLCConnectorModule;
            Temp.ComObjectRequested += NavToComObjectModule;
            CurrentPage = Temp;
            CurrentPageName = "PLC Connector Module";
            CurrentPage.IsRightsMode = true; //volani permission modulu

        }

        private void NavToAddEditPLCConnectorModule(PLCConnectorModuleConfigure p_obj)
        {
            // jeste zjistit jestli na to ma uzivatel prava
            AddEditPLCConnectorViewModel Temp = new AddEditPLCConnectorViewModel(p_obj);
            Temp.Done += NavToPLCConnectorModule;
            CurrentPage = Temp;
            CurrentPageName = "Add/Edit PLC Connector Module";
            CurrentPage.IsRightsMode = true; //volani permission modulu

        }
        private void NavToComObjectModule(Guid p_idGuid)
        {
            // jeste zjistit jestli na to ma uzivatel prava
            ComObjectViewModel Temp = new ComObjectViewModel(p_idGuid);
            Temp.AddEditComObjectRequested += NavToAddEditComObjectModule;
            CurrentPage = Temp;
            CurrentPageName = "ComObject Module";
            CurrentPage.IsRightsMode = true; //volani permission modulu

        }

        private void NavToAddEditComObjectModule(ComObjectConfigure p_obj)
        {
            // jeste zjistit jestli na to ma uzivatel prava
            AddEditComObjectViewModel Temp = new AddEditComObjectViewModel(p_obj);
            Temp.Done += NavToComObjectModule;
            CurrentPage = Temp;
            CurrentPageName = "Add/Edit ComObject Module";
            CurrentPage.IsRightsMode = true; //volani permission modulu

        }

        private void NavToEntity()
        {
            // jeste zjistit jestli na to ma uzivatel prava
            EntitiesViewModel Temp = new EntitiesViewModel();
            Temp.AddEditEntityRequested += NavToAddEditEntity;
            Temp.StatesEntityRequested += NavToState;
            CurrentPage = Temp;
            CurrentPageName = "Entities";
            CurrentPage.IsRightsMode = true; //volani permission modulu

        }

        private void NavToAddEditEntity(EntityModel p_entity)
        {
            // jeste zjistit jestli na to ma uzivatel prava
            AddEditEntitiesViewModel Temp = new AddEditEntitiesViewModel(p_entity);
            Temp.Done += NavToEntity;
            CurrentPage = Temp;
            CurrentPageName = "Add/Edit Page";
            CurrentPage.IsRightsMode = true; //volani permission modulu
        }

        private void NavToState(int p_entitid) // musi vstupovat string ne objekt
        {
            // jeste zjistit jestli na to ma uzivatel prava
            StateEntitiesViewModel Temp = new StateEntitiesViewModel(p_entitid);
            Temp.AddEditStateRequested += NavToAddEditState;
            Temp.TranslationRequested += NavToTranslation;
            CurrentPage = Temp;
            CurrentPageName = "States";
            CurrentPage.IsRightsMode = true; //volani permission modulu
        }

        private void NavToAddEditState(StateModel p_state)
        {
            AddEditStateViewModel Temp = new AddEditStateViewModel(p_state);
            Temp.Done += NavToState;
            CurrentPage = Temp;
            CurrentPageName = "States";
            CurrentPage.IsRightsMode = true; //volani permission modulu
        }


        private void NavToTranslation(int p_entitid) // ID stavu bude vstupovat ze ktereho chci vyjít
        {
            // jeste zjistit jestli na to ma uzivatel prava
            TranslationStateViewModel Temp = new TranslationStateViewModel(p_entitid);
            Temp.AddEditTranslationRequested += NavToAddEditTranslation;
            CurrentPage = Temp;
            CurrentPageName = "States";
            CurrentPage.IsRightsMode = true; //volani permission modulu
        }

        private void NavToAddEditTranslation(TranslationModel p_translation)
        {
            AddEditTranslationViewModel Temp = new AddEditTranslationViewModel(p_translation);
            Temp.Done += NavToTranslation;
            CurrentPage = Temp;
            CurrentPageName = "States";
            CurrentPage.IsRightsMode = true; //volani permission modulu
        }


        #endregion


        #region Commands

        private bool CanDoLogout()
        {
            return AuthVM.IsAuthenticated;
        }

        private void DoLogout()
        {
            AuthVM.LogOff();
        }

        #endregion

        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private System.Windows.Window mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 400;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get { return Borderless ? 0 : 6; } }

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;
        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }
        #endregion
        #region Commands

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public RelayCommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public RelayCommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        public RelayCommand MenuCommand { get; set; }

        #endregion


        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(mWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            RaisePropertyChanged(nameof(Borderless));
            RaisePropertyChanged(nameof(ResizeBorderThickness));
            RaisePropertyChanged(nameof(OuterMarginSize));
            RaisePropertyChanged(nameof(OuterMarginSizeThickness));
            RaisePropertyChanged(nameof(WindowRadius));
            RaisePropertyChanged(nameof(WindowCornerRadius));
        }


        #endregion
    }




}
