using MahApps.Metro.Controls.Dialogs;
using EventsProject.Domain.Common;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using MahApps.Metro.Controls;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Presentation.DTOs;
using System.Windows;
using System.Windows.Media;

namespace EventsProject.Presentation.UI.Forms;

public partial class EnrrollmentEventForm : CustomDialog {
    //------------------------INITIALIZATION------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly EventDTO _event;
    private readonly UserDTO _user;
    private int _boughtTickets;

    public EnrrollmentEventForm(IDataService dataService, IMainWindowService mainWindowService, UserDTO user, EventDTO eventInfo) : 
        base((MetroWindow)mainWindowService) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;
        _event = eventInfo;
        LoadVisualInfo();
    }

    //------------------------METHODS------------------------
    //------------------------btnMeths------------------------
    private async void btnBack_Click(object sender, RoutedEventArgs e) => await _mainWindowService.CloseDialogAsync(this);
    private async void btnEnrroll_Click(object sender, RoutedEventArgs e) {
        int ticketsAmount = (int)(nudCapacity.Value ?? 0);

        if (ticketsAmount < 1 || ticketsAmount > _event.Capacity - _boughtTickets) {
            _mainWindowService.ShowNotification("Purchase failed\nAmount of tickets is out of range", EnumNotifierType.Error);
            return;
        }

        Result enrrollResult = await _dataService.UserEventService.EnrrollUserInAsync(_user.UserId, _event.EventId, ticketsAmount);
        if (!enrrollResult.Success) {
            _mainWindowService.ShowNotification($"Purchase failed\n{enrrollResult.Description}", EnumNotifierType.Error);
            return;
        }
          
        await _mainWindowService.ShowMsgAsync($"Congratulations!, you just purchased {ticketsAmount} tickets to {_event.Title}," +
            $" be aware of the event's date");

        await _dataService.NotificationService.CreateNotificationAsync(EnumNotificationType.Enrollment, _user.UserId, _event.Title, ticketsAmount);

        await _mainWindowService.CloseDialogAsync(this);
    }
    
    //------------------------innerMeths------------------------
    private void LoadVisualInfo() {
        spEventContent.DataContext = _event;
        _boughtTickets = _dataService.UserEventService.EnrrolledInEventCount(_event.EventId);
        lblBougthAmount.Content = _boughtTickets;
        bdEventState.Background = _event.EventState == EnumEventState.Active ? new SolidColorBrush(Colors.Green) 
                                    : new SolidColorBrush(Colors.Red);
        bool enabledInteraction = _event.EventState == EnumEventState.Active;
        btnEnrroll.IsEnabled = enabledInteraction;
        nudCapacity.IsEnabled = enabledInteraction;
    }
}
