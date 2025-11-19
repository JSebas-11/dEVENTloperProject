using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Text;
using System.Windows;
using ValidationHelper = EventsProject.Domain.Common.ValidationHelper;
using EventsProject.Presentation.UI.Pages;

namespace EventsProject.Presentation.UI.Forms;

public partial class RestorePasswordForm : CustomDialog {
    //------------------------INITIALIZATION------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserAccount _user;

    public RestorePasswordForm(IDataService dataService, IMainWindowService mainWindowService, UserAccount user) : 
        base((MetroWindow)mainWindowService) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;
        LoadVisualInfo();
    }

    //------------------------METHODS------------------------
    //------------------------btnMeths------------------------
    private async void btnBack_Click(object sender, RoutedEventArgs e) => await _mainWindowService.CloseDialogAsync(this);
    private async void btnConfirmPassword_Click(object sender, RoutedEventArgs e) {
        string password = pwbPassword.Password.Trim();
        string confirmPassword = pwbPasswordConfirm.Password.Trim();

        if (!string.Equals(password, confirmPassword)) {
            _mainWindowService.ShowNotification("Password Invalid\nPassword and its confirmation don't match", EnumNotifierType.Warning);
            return;
        }

        Result passwordVerification = VerifyPassword(password);
        if (!passwordVerification.Success) {
            _mainWindowService.ShowNotification($"Password Invalid\n{passwordVerification.Description}", EnumNotifierType.Warning);
            return;
        }

        //Aplicar cambios al usuario
        ApplyChanges(password);
        Result updateResult = await _dataService.UserService.UpdateUserAsync(_user);
        await _mainWindowService.ShowMsgAsync(updateResult.Description);

        if (!updateResult.Success) { return; }

        await _mainWindowService.CloseDialogAsync(this);
        _mainWindowService.NavigateTo(new LoginPage(_dataService, _mainWindowService));
    }

    //------------------------innerMeths------------------------
    private void ApplyChanges(string password)
        => _user.HashPassword = _dataService.UtilitiesService.HashPassword(password);
    private void LoadVisualInfo() => lblUserName.Content = _user.UserName;
    private Result VerifyPassword(string password) {
        var errors = new StringBuilder();

        ValidationHelper.ValidateNull(password, "Password", errors);
        ValidationHelper.ValidatePassword(password, errors);

        //Devolver el result con el mensaje y resultado correpondiente
        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }
}
