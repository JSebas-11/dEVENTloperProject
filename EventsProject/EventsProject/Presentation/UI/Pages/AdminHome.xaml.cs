using EventsProject.Domain.Abstractions.Services;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.Mappers;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.UI.Pages;

public partial class AdminHome : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _admin;

    public AdminHome(IDataService dataService, IMainWindowService mainWindowService, UserDTO admin) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _admin = admin;

        Loaded += AdminHome_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns Meths-------------------------
    private async void btnRefresh_Click(object sender, RoutedEventArgs e) => await LoadData();
    private async void btnExecuteQuery_Click(object sender, RoutedEventArgs e) {
        string query = txbQuery.Text;
        if (!ValidQuery(query)) {
            _mainWindowService.ShowNotification("Query Error\nThere has been an error in query, " +
                "check your query out:\n-Empty input\n-Only SELECT queries are allowed", EnumNotifierType.Warning);
            return;
        }

        DataTable? queryResult = await _dataService.CustomizedQueryAsync(query);
        if (queryResult == null) {
            _mainWindowService.ShowNotification("Query Info\nQuery result was null");
            return;
        }
        dgQuery.ItemsSource = queryResult.DefaultView;
    }

    private void btnClearQuery_Click(object sender, RoutedEventArgs e) => txbQuery.Text = string.Empty;

    //-------------------------inner Meths-------------------------
    private async void AdminHome_Loaded(object sender, RoutedEventArgs e) {
        try {
            gdContent.DataContext = _admin;
            lblAdminName.Content = $"Hello!, {_admin.UserName}";
            await LoadData();
        }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading Metrics:\n{ex.Message}", EnumNotifierType.Error);
        }
    }

    private async Task LoadData() {
        //Asignacion de datos a elementos visuales
        scTotalUsers.Value = (await _dataService.DataMetrics.TotalUsersAsync()).ToString();
        scTotalEvents.Value = (await _dataService.DataMetrics.TotalEventsAsync()).ToString();
        scActiveEvents.Value = (await _dataService.DataMetrics.ActiveEventsCountAsync()).ToString();
        scActiveEvents.Value = (await _dataService.DataMetrics.EventsCountByDateAsync(DateTime.Today)).ToString();

        containerEvent.DataContext = await _dataService.DataMetrics.MostEnrolledEventAsync();
        containerCity.DataContext = await _dataService.DataMetrics.MostEventsCityAsync();
        containerCategory.DataContext = await _dataService.DataMetrics.MostEventsCategoryAsync();
    }
    private bool ValidQuery(string query) {
        query = query.Trim().ToUpper();
        return !string.IsNullOrWhiteSpace(query)
            && Regex.IsMatch(query, @"SELECT\s+.+\s+FROM\s+.+;?", RegexOptions.IgnoreCase);
    }
}
