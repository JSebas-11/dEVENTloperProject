using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.Common;
using System.Windows;
using System.Windows.Controls;
using EventsProject.Presentation.Mappers;

namespace EventsProject.Presentation.UI.Pages;

public partial class UserHome : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _user;

    public UserHome(IDataService dataService, IMainWindowService mainWindowService, UserDTO user) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;

        Loaded += UserHome_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns Meths-------------------------
    private async void btnRefresh_Click(object sender, RoutedEventArgs e) => await LoadData();
    private async void btnMarkAsRead_Click(object sender, RoutedEventArgs e) {
        if (sender is Button btn) {
            NotificationInfo? notification = btn.Tag as NotificationInfo;
            if (notification is null) return;

            Result result = await _dataService.NotificationService.ChangeNotificationStateAsync(notification, EnumNotificationState.Read);
            
            if (!result.Success) {
                _mainWindowService.ShowNotification($"Notification Info\nNotification id ({notification.NotificationId}) " +
                    $"was not able to be marked as read"
                );
                return;
            }
                    
            await LoadNotifications();
            _mainWindowService.ShowNotification($"Notification Info\nNotification id ({notification.NotificationId}) was marked as read", 
                EnumNotifierType.Success
            );
        }
    }

    //-------------------------inner Meths-------------------------
    private async void UserHome_Loaded(object sender, RoutedEventArgs e) {
        try {
            gdContent.DataContext = _user;
            lblUserName.Content = $"Hello!, {_user.UserName}";
            await LoadData();
        }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading User Data:\n{ex.Message}", EnumNotifierType.Error);
        }
    }

    private async Task LoadData() {
        int userId = _user.UserId;
        //Asignacion de datos a elementos visuales
        //Metrics
        lblEnrolledEvents.Content = await _dataService.DataMetrics.EnrollmentsCountByUserAsync(userId);
        lblCreatedEvents.Content = await _dataService.DataMetrics.CreatedEventsCountByUserAsync(userId);
        lblPendingEvents.Content = await _dataService.DataMetrics.PendingEventsCountByUserAsync(userId);
        //Notifications
        await LoadNotifications();
        //Events
        await LoadEvents(userId);
    }
    
    private async Task LoadEvents(int userId) {
        List<EventInfo> lastEvents = await _dataService.UserEventService.GetLastNEventByUserAsync(userId);
        List<EventInfo> nextEvents = await _dataService.UserEventService.GetNextNEventByUserAsync(userId);

        icLastEvents.ItemsSource = await EventMapper.ToDTOListAsync(lastEvents, _dataService.ConverterService);
        icNextEvents.ItemsSource = await EventMapper.ToDTOListAsync(nextEvents, _dataService.ConverterService);
    }
    
    private async Task LoadNotifications() {
        int userId = _user.UserId;
        //Notifications
        var unreadNotfs = await _dataService.NotificationService.GetNoReadNotificationsByUserAsync(userId);
        icNotification.ItemsSource = unreadNotfs;
        lblNotification.Content = $" {unreadNotfs.Count} ";
    }

}
