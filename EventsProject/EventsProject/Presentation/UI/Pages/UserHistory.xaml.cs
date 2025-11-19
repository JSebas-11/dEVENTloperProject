using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.Mappers;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Pages;

public partial class UserHistory : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _user;
    
    public UserHistory(IDataService dataService, IMainWindowService mainWindowService, UserDTO user) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;
        LoadVisualInfo();

        Loaded += UserHistory_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------innerMeths-------------------------
    private async void UserHistory_Loaded(object sender, RoutedEventArgs e) {
        try {
            //Obtener Eventos desde el servicio
            List<EventInfo> enrolledEvents = await _dataService.UserEventService.GetUniqueEnrolledEventsByUserAsync(_user.UserId);
            List<EventInfo> createdEvents = await _dataService.EventService.GetEventsCreatedByIdAsync(_user.UserId);

            //Mappear Eventos del modelo a EventDTO
            List<EventDTO> enrolledEventsDTO = await EventMapper.ToDTOListAsync(enrolledEvents, _dataService.ConverterService);
            List<EventDTO> createdEventsDTO = await EventMapper.ToDTOListAsync(createdEvents, _dataService.ConverterService);

            //Ordenarlos y asignarlos al ItemControl de la UI
            icEnrolledEvents.ItemsSource = enrolledEventsDTO.OrderByDescending(ev => ev.InitialTime);
            icCreatedEvents.ItemsSource = createdEventsDTO.OrderByDescending(ev => ev.InitialTime);
        }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading History Data:\n{ex.Message}", EnumNotifierType.Error);
        }
    }
    
    private void LoadVisualInfo() {
        gdMainContainer.DataContext = _user;
        lblUserName.Content = $"{_user.UserName}'s events history";
    }
}
