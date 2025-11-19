using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Infrastructure;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.DTOs;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

namespace EventsProject.Presentation.UI.Forms;

public partial class CreationEventForm : CustomDialog {
    //------------------------INITIALIZATION------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserDTO _user;
    private string? _eventImgPath = null;

    public CreationEventForm(IDataService dataService, IMainWindowService mainWindowService, UserDTO user) : 
        base((MetroWindow)mainWindowService) 
    {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;
        InitVisualComponents();
    }

    //------------------------METHODS------------------------
    //------------------------btnMeths------------------------
    private async void btnBack_Click(object sender, RoutedEventArgs e) => await _mainWindowService.CloseDialogAsync(this);
    private void btnImgDialog_Click(object sender, RoutedEventArgs e) => FileExplorerDialog();

    private async void btnConfirm_Click(object sender, RoutedEventArgs e) {
        //Comprobacion de inputs obligatorios
        Result errorsResult = _dataService.EventService.ValidateEventFields(
            false, txbTitle.Text.Trim(), (int?)nudCapacity.Value, (int?)cmbCategory.SelectedValue, txbArtist.Text.Trim(),
            txbDescription.Text.Trim(), dtpInitalHour.SelectedDateTime, dtpEndHour.SelectedDateTime,
            txbPlace.Text.Trim(), txbCity.Text.Trim()
        );
        if (!errorsResult.Success) {
            await _mainWindowService.ShowMsgAsync(errorsResult.Description);
            Background = (Brush)FindResource("linerGradBG");
            return;
        }

        //Confirmacion de creacion sin imagen
        if (_eventImgPath == null) {
            MessageDialogResult dialogResult = await _mainWindowService.ShowMsgAsync(
                "Do you want to create the event without image? (Default image will be assigned)", MessageDialogStyle.AffirmativeAndNegative
            );
            Background = (Brush)FindResource("linerGradBG");

            if (dialogResult == MessageDialogResult.Negative) return;
        }

        //Crear evento y mostrar resultado
        Result result = await _dataService.EventService.CreateEventAsync(
            txbTitle.Text, (int)nudCapacity.Value, (int)cmbCategory.SelectedValue, txbArtist.Text, txbDescription.Text,
            dtpInitalHour.SelectedDateTime.Value, dtpEndHour.SelectedDateTime.Value,
            txbPlace.Text, txbCity.Text, _user.UserId, _user.IsAdmin, _eventImgPath
        );

        await _mainWindowService.ShowMsgAsync(result.Description);
        Background = (Brush)FindResource("linerGradBG");
        if (result.Success)
            await _mainWindowService.CloseDialogAsync(this);
    }

    //------------------------innerMeths------------------------
    private async Task InitVisualComponents() {
        string formats = string.Empty;
        Config.validExtensions.ForEach(ext => formats += $"{ext} ");
        lblFormats.Content = $"Allowed formats: {formats}";
        
        //Category ComBox definition
        cmbCategory.ItemsSource = await _dataService.CategoryService.GetCategoriesAsync();
        cmbCategory.DisplayMemberPath = "CatName";
        cmbCategory.SelectedValuePath = "CatId";
        
        //Date TimePickers definition
        dtpInitalHour.DisplayDate = DateTime.Today;
        dtpInitalHour.DisplayDateStart = DateTime.Today;
        dtpEndHour.DisplayDate = DateTime.Today;
        dtpEndHour.DisplayDateStart = DateTime.Today;
    }
    private void FileExplorerDialog() {
        var opfDialog = new OpenFileDialog() { Filter = "Images|*.png;*.jpg;*.jpeg" };

        if (opfDialog.ShowDialog() == true) _eventImgPath = opfDialog.FileName;
    }
}
