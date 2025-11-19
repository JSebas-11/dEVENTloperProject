using EventsProject.Domain.Abstractions.Services;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.UI.Pages;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Containers;
public enum AdminNavigation { Home, Managment, Request, Exit }

public partial class AdminLayout : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _admin;
    
    public AdminLayout(IDataService dataService, IMainWindowService mainWindowService, UserDTO admin) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _admin = admin;
    }

    //-------------------------METHODS-------------------------
    //-------------------------navigation-------------------------
    private async void hmAdmin_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args) {
        //Comprobar y recibir los enum contenidos en los Tags
        if (args.InvokedItem is HamburgerMenuIconItem item) {
            switch (item.Tag) {
                //Navegacion en la AdminPage
                case AdminNavigation.Home:
                    NavigateTo(new AdminHome(_dataService, _mainWindowService, _admin));
                    break;
                case AdminNavigation.Managment:
                    NavigateTo(new AdminManag(_dataService, _mainWindowService, _admin));
                    break;
                case AdminNavigation.Request:
                    NavigateTo(new AdminRequest(_dataService, _mainWindowService));
                    break;

                //Navegacion en la ventana principal
                case AdminNavigation.Exit:
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
        frAdmin.Navigate(page);
        while (frAdmin.NavigationService.CanGoBack) {
            //Eliminar historial de navegacion para evitar cargas en memoria
            frAdmin.NavigationService.RemoveBackEntry();
        }
    }

}
