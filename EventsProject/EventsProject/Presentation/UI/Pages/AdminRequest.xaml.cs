using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Pages;

public partial class AdminRequest : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;

    //-------------------------editProps-------------------------
    public enum EnumEventAction { NoAction, Approve, Deny };
    public static List<EnumEventAction> EventActions { get; } = [EnumEventAction.NoAction, EnumEventAction.Approve, EnumEventAction.Deny];
    private readonly ObservableCollection<EventInfo> _approvedEvents = [];
    private readonly ObservableCollection<EventInfo> _deniedEvents = [];

    public AdminRequest(IDataService dataService, IMainWindowService mainWindowService) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;

        Loaded += AdminRequest_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns-events methods-------------------------
    private async void btnRefresh_Click(object sender, RoutedEventArgs e) {
        await LoadDataAsync();
        await LoadVisualInfoAsync();
    }

    private async void btnSaveChanges_Click(object sender, RoutedEventArgs e) {
        if (_approvedEvents.Count == 0 && _deniedEvents.Count == 0) {
            _mainWindowService.ShowNotification("Update Info\nThere is no changes to apply");
            return;
        }

        var infoMsg = new StringBuilder($"Changes information:\n-Approved Events (id-result): \n");
        foreach (EventInfo evnt in _approvedEvents) { 
            Result updateResult = await _dataService.EventService.ChangeEventStateAsync(evnt, (int)EnumEventState.Active);
            infoMsg.Append($"{evnt.EventId} - {(updateResult.Success ? "Succesfully " : "Error ")}");

            if (updateResult.Success) await _dataService.NotificationService.CreateNotificationAsync(EnumNotificationType.Approved, (int)evnt.CreatedById, evnt.Title);
        }
        infoMsg.AppendLine("\n-Denied Events (id-result): ");
        foreach (EventInfo evnt in _deniedEvents) { 
            Result updateResult = await _dataService.EventService.ChangeEventStateAsync(evnt, (int)EnumEventState.Cancelled);
            infoMsg.Append($"{evnt.EventId} - {(updateResult.Success ? "Succesfully " : "Error ")}");
            
            if (updateResult.Success) await _dataService.NotificationService.CreateNotificationAsync(EnumNotificationType.Rejected, (int)evnt.CreatedById, evnt.Title);
        }

        await _mainWindowService.ShowMsgAsync(infoMsg.ToString());

        _approvedEvents.Clear();
        _deniedEvents.Clear();
        await LoadDataAsync();
        await LoadVisualInfoAsync();
    }

    private void EventAprobation_ApproveClicked(object sender, EventInfo e) {
        if (_approvedEvents.Any(ev => ev.EventId == e.EventId)) return;
        _deniedEvents.Remove(e);
        _approvedEvents.Add(e);
        UpdateEventsDgCount();
    }

    private void EventAprobation_DenyClicked(object sender, EventInfo e) {
        if (_deniedEvents.Any(ev => ev.EventId == e.EventId)) return;
        _approvedEvents.Remove(e);
        _deniedEvents.Add(e);
        UpdateEventsDgCount();
    }

    //-------------------------innerMeths-------------------------
    private async void AdminRequest_Loaded(object sender, RoutedEventArgs e) {
        try {
            dgApprovedEvents.ItemsSource = _approvedEvents;
            dgRejectedEvents.ItemsSource = _deniedEvents;
            await LoadDataAsync();
            await LoadVisualInfoAsync();
        }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading Requested Events:\n{ex.Message}", EnumNotifierType.Error);
        }
    }

    private async Task LoadVisualInfoAsync() {
        lblPendingEvents.Content = await _dataService.DataMetrics.EventsCountByStateAsync(EnumEventState.Pending);
        UpdateEventsDgCount();
    }
    private async Task LoadDataAsync() {
        //Filtrar eventos por estado Pendiente
        List<EventInfo> pendingEvents = await _dataService.FilterService.FilterEventsAsync(
            new EventFilterOptions(EnumEvenFilterOptions.State, ((int)EnumEventState.Pending).ToString())
        );
        icRequestedEvents.ItemsSource = pendingEvents.OrderBy(ei => ei.InitialTime).ToList();
        _approvedEvents.Clear();
        _deniedEvents.Clear();
        UpdateEventsDgCount();
    }
    private void UpdateEventsDgCount() {
        lblApprovedCount.Content = _approvedEvents.Count;
        lblRejectedCount.Content = _deniedEvents.Count;
    }
}
