using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.UI.Forms;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Pages;

public partial class AdminManag : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _admin;
    private bool _isCommiting = false;
    public AdminManag(IDataService dataService, IMainWindowService mainWindowService, UserDTO admin) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _admin = admin;

        Loaded += AdminManag_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns-events methods-------------------------
    private async void btnRefresh_Click(object sender, RoutedEventArgs e) => await LoadDataAsync();

    private async void btnFilter_Click(object sender, RoutedEventArgs e) {
        string filterValue = string.Empty;
        EnumEvenFilterOptions filterField = (EnumEvenFilterOptions)cmbFilters.SelectedItem;

        switch (filterField) {
            case EnumEvenFilterOptions.All:
                await LoadDataAsync();
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
            case EnumEvenFilterOptions.State:
                filterValue = cmbFilterInput.SelectedValue.ToString();
                break;
        }

        List<EventInfo> eventsResult = await _dataService.FilterService.FilterEventsAsync(new EventFilterOptions(filterField, filterValue));
        dgEvents.ItemsSource = eventsResult;
        lblNumResults.Content = eventsResult.Count;
    }

    private async void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        => await _mainWindowService.ShowDialogAsync(new CreationEventForm(_dataService, _mainWindowService, _admin));

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

            case EnumEvenFilterOptions.State:
                cmbFilterInput.Visibility = Visibility.Visible;
                cmbFilterInput.ItemsSource = await _dataService.EventStateService.GetEventStatesAsync();
                cmbFilterInput.SelectedValuePath = "EventStateId";
                cmbFilterInput.DisplayMemberPath = "StateName";
                cmbFilterInput.SelectedIndex = 0;
                break;
        }
    }

    private async void dgEvents_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) {
        //Metodo para gestionar los updates del DataGrid de los eventos
        if (e.EditAction == DataGridEditAction.Commit) {
            if (_isCommiting) return;
            _isCommiting = true;
            
            //Forzar commit en el DataGrid para obtener el objeto actualizado
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);

            // Obtener el objeto editado
            if (e.Row.Item is not EventInfo editedEvent) {
                _isCommiting = false;
                return;
            }

            Result validationResult = _dataService.EventService.ValidateEventFields(
                true, editedEvent.Title.Trim(), editedEvent.Capacity, editedEvent.CatId,
                editedEvent.Artist.Trim(), editedEvent.EventDescription.Trim(), 
                editedEvent.InitialTime, editedEvent.EndTime, 
                editedEvent.EventPlace, editedEvent.EventCity, editedEvent.EventStateId
            );

            if (!validationResult.Success) {
                await _mainWindowService.ShowMsgAsync(validationResult.Description);
                return;
            }

            Result updateResult = await _dataService.EventService.UpdateEventAsync(editedEvent);

            EnumNotifierType type = updateResult.Success ? EnumNotifierType.Success : EnumNotifierType.Info;
            _mainWindowService.ShowNotification($"Update Result\n{updateResult.Description}", type);

            _isCommiting = false;
        }
    }

    //-------------------------innerMeths-------------------------
    private async void AdminManag_Loaded(object sender, RoutedEventArgs e) {
        try { await LoadDataAsync(); }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading Events Data:\n{ex.Message}", EnumNotifierType.Error);
        }
    }

    private async Task LoadDataAsync() {
        cmbFilters.ItemsSource = _dataService.FilterService.AvailableFilters;
        cmbFilters.SelectedIndex = 0;

        List<EventInfo> events = await _dataService.EventService.GetEventsAsync();
        dgEvents.ItemsSource = events;
        lblNumResults.Content = events.Count;

        // Ejecutar la funcion si ItemsSource is null
        dgEventsCmbCategory.ItemsSource ??= await _dataService.CategoryService.GetCategoriesAsync();
        dgEventsCmbState.ItemsSource ??= await _dataService.EventStateService.GetEventStatesAsync();
    }
}
