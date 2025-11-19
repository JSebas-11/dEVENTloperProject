using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.Mappers;
using EventsProject.Presentation.UI.Forms;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Pages;

public partial class UserEvents : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _user;
    
    public UserEvents (IDataService dataService, IMainWindowService mainWindowService, UserDTO user) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;

        Loaded += UserEvents_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns-events methods-------------------------
    private async void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        => await _mainWindowService.ShowDialogAsync(new CreationEventForm(_dataService, _mainWindowService, _user));

    private async void EventEnrollControl_EventClicked(object sender, EventDTO e) {
        var enrrollDialog = new EnrrollmentEventForm(_dataService, _mainWindowService, _user, e);
        await _mainWindowService.ShowDialogAsync(enrrollDialog);
    }
    private async void btnFilter_Click(object sender, RoutedEventArgs e) {
        string filterValue = string.Empty;
        EnumEvenFilterOptions filterField = (EnumEvenFilterOptions)cmbFilters.SelectedItem;

        switch (filterField) {
            case EnumEvenFilterOptions.All:
                await LoadEventsAsync();
                return;

            case EnumEvenFilterOptions.Title:
            case EnumEvenFilterOptions.City:
            case EnumEvenFilterOptions.Place:
                filterValue = txbFilterInput.Text.Trim();
                break;

            case EnumEvenFilterOptions.Date:
                if (dtpDateFilter.SelectedDateTime is DateTime selectedDate)
                    filterValue = selectedDate.ToString("yyyy-MM-dd");
                break;

            case EnumEvenFilterOptions.Category:
                filterValue = cmbFilterInput.SelectedValue.ToString();
                break;
        }

        List<EventInfo> eventsResult = await _dataService.FilterService.FilterEventsWithCategoriesAsync(new EventFilterOptions(filterField, filterValue));
        eventsResult = GetEventsToShow(eventsResult);
        icEvents.ItemsSource = await MapEvents(eventsResult);
        lblNumResults.Content = eventsResult.Count;
    }

    private async void cmbFilters_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        //Mostrar el input correspondiente de acuerdo al campo de filtro seleccionado
        EnumEvenFilterOptions filterField = (EnumEvenFilterOptions)cmbFilters.SelectedItem;
        txbFilterInput.Visibility = Visibility.Collapsed;
        dtpDateFilter.Visibility = Visibility.Collapsed;
        cmbFilterInput.Visibility = Visibility.Collapsed;

        switch (filterField) {

            case EnumEvenFilterOptions.Title:
            case EnumEvenFilterOptions.City:
            case EnumEvenFilterOptions.Place:
                txbFilterInput.Visibility = Visibility.Visible;
                break;

            case EnumEvenFilterOptions.Date:
                dtpDateFilter.Visibility = Visibility.Visible;
                break;

            case EnumEvenFilterOptions.Category:
                cmbFilterInput.Visibility = Visibility.Visible;
                cmbFilterInput.ItemsSource = await _dataService.CategoryService.GetCategoriesAsync();
                cmbFilterInput.SelectedValuePath = "CatId";
                cmbFilterInput.DisplayMemberPath = "CatName";
                cmbFilterInput.SelectedIndex = 0;
                break;
        }
    }

    //-------------------------innerMeths-------------------------
    private async void UserEvents_Loaded(object sender, RoutedEventArgs e) {
        try {
            List<EnumEvenFilterOptions> avFilters = _dataService.FilterService.AvailableFilters.ToList();
            avFilters.RemoveAll(fil => fil == EnumEvenFilterOptions.State);
            cmbFilters.ItemsSource = avFilters;
            cmbFilters.SelectedIndex = 0;
            await LoadEventsAsync();
        }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading User Info:\n{ex.Message}", EnumNotifierType.Error);
        }
    }

    private List<EventInfo> GetEventsToShow(List<EventInfo> events)
        //Filtrado de eventos con estados de activoa o soldOut
        => events.Where(e => e.EventStateId == (int)EnumEventState.Active || e.EventStateId == (int)EnumEventState.SoldOut)
                .ToList();
    
    private async Task<List<EventDTO>> MapEvents(List<EventInfo> events)
        => await EventMapper.ToDTOListAsync(events, _dataService.ConverterService);
    
    private async Task LoadEventsAsync() {
        var filteredEvents = GetEventsToShow(await _dataService.EventService.GetEventsWithCartegoriesAsync());
        var events = await MapEvents(filteredEvents);
        icEvents.ItemsSource = events;
        lblNumResults.Content = $" {events.Count} ";
    }
}
