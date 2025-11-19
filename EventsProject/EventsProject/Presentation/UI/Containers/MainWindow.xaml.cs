using EventsProject.Domain.Abstractions.Services;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using EventsProject.Presentation.UI.Pages;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EventsProject.Presentation.UI.Containers;

public partial class MainWindow : MetroWindow, IMainWindowService {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private NotifierService _notifier;

    public MainWindow(IDataService dataService) {
        InitializeComponent();
        _dataService = dataService;
        Loaded += MainWindow_Loaded;
    }

    //-------------------------METHODS-------------------------
    public void NavigateTo(Page page) {
        frMain.Navigate(page);
        while (frMain.NavigationService.CanGoBack) {
            //Eliminar historial de navegacion para evitar cargas en memoria
            frMain.NavigationService.RemoveBackEntry();
        }
    }

    public void ShowNotification(string msg, EnumNotifierType type = EnumNotifierType.Info)
        => _notifier.Show(msg, type);

    public async Task ShowDialogAsync(CustomDialog dialog) => await this.ShowMetroDialogAsync(dialog);
    public async Task CloseDialogAsync(CustomDialog dialog) => await this.HideMetroDialogAsync(dialog);

    public void CloseWindow() => this.Close();
    public Task<MessageDialogResult> ShowMsgAsync(string msg, MessageDialogStyle msgDialogStyle = MessageDialogStyle.Affirmative, string title = "INFORMATION") 
        => this.ShowMessageAsync(title, msg, msgDialogStyle);
    public async Task GenerateFloatToolTipAsync(TextBox txb) {
        //Crear tooltip junto con el TextBox accionante
        var toolTip = new ToolTip {
            Content = txb.ToolTip,
            IsOpen = true,
            PlacementTarget = txb,
            Placement = PlacementMode.Relative,
            StaysOpen = false
        };

        //Cerrar automáticamente despues de 1 segundo
        await Task.Delay(1000);
        toolTip.IsOpen = false;
    }
    
    protected override async void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        _notifier.Dispose();
        await _dataService.DisposeAsync();
    }
    
    //-------------------------innerMeths-------------------------
    private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
        _notifier = new NotifierService(this);
        NavigateTo(new LoginPage(_dataService, (IMainWindowService)this));
    }
}