using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.UI.Pages;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Containers;

public enum UserNavigation { Home, Events, History, Edit, Exit }
public partial class UserLayout : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _user;
    private UserAccount _userModel;

    public UserLayout(IDataService dataService, IMainWindowService mainWindowService, UserDTO user) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;

        Loaded += UserLayout_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------navigation-------------------------
    private async void hmUser_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args) {
        //Comprobar y recibir los enum contenidos en los Tags
        if (args.InvokedItem is HamburgerMenuIconItem item) {
            switch (item.Tag) {
                //Navegacion en la AdminPage
                case UserNavigation.Home:
                    NavigateTo(new UserHome(_dataService, _mainWindowService, _user));
                    break;
                case UserNavigation.Events:
                    NavigateTo(new UserEvents(_dataService, _mainWindowService, _user));
                    break;
                case UserNavigation.History:
                    NavigateTo(new UserHistory(_dataService, _mainWindowService, _user));
                    break;
                case UserNavigation.Edit:
                    NavigateTo(new UserEdit(_dataService, _mainWindowService, _userModel, _user.UserImg));
                    break;

                //Navegacion en la ventana principal
                case UserNavigation.Exit:
                    var result = await _mainWindowService.ShowMsgAsync("Are you sure do you want to close your session?", MessageDialogStyle.AffirmativeAndNegative);
                    if (result == MessageDialogResult.Affirmative)
                        _mainWindowService.NavigateTo(new LoginPage(_dataService, _mainWindowService));
                    break;

                default:
                    break;
            }
        }
    }

    //-------------------------inner Methods-------------------------
    private void NavigateTo(Page page) {
        frUser.Navigate(page);
        while (frUser.NavigationService.CanGoBack) {
            //Eliminar historial de navegacion para evitar cargas en memoria
            frUser.NavigationService.RemoveBackEntry();
        }
    }

    private async void UserLayout_Loaded(object sender, RoutedEventArgs e) {
        UserAccount? user = await _dataService.UserService.GetUserAsync(_user.Dni);
        if (user is null) {
            await _mainWindowService.ShowMsgAsync("User Model couldn't be loaded, you will be redirected to Login Page");
            NavigateTo(new LoginPage(_dataService, _mainWindowService));
            return;
        }

        _userModel = user;
    }
}