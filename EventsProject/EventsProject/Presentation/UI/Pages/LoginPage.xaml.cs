using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.DTOs;
using EventsProject.Presentation.Mappers;
using EventsProject.Presentation.Common;
using EventsProject.Presentation.UI.Containers;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventsProject.Presentation.UI.Pages;

public partial class LoginPage : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private string _userImgPath;

    public LoginPage(IDataService dataService, IMainWindowService mainWindowService) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns methods-------------------------
    private async void btnLog_Click(object sender, RoutedEventArgs e) {
        Result validation = ValidLogin();
        if (!validation.Success) {
            await _mainWindowService.ShowMsgAsync(validation.Description);
            return;
        }

        btnLog.IsEnabled = false;
        //Obtener usuario y en caso de no existir informamos a usuario y salimos del metodo
        UserAccount? user = await _dataService.UserService.GetUserAsync(txbLgDNI.Text, pwbLgPassword.Password);
        btnLog.IsEnabled = true;
        if (user == null) {
            await _mainWindowService.ShowMsgAsync("DNI and password don't match. Try again");
            return;
        }

        //Navegar a menu correspondiente mappeando al DTO (User o Admin)
        UserDTO userDTO = UserMapper.ToDTO(user, _dataService.ConverterService);
        _mainWindowService.ShowNotification($"Allowed Access!\nWelcome again {user.UserName}", EnumNotifierType.Success);
        if (user.IsAdmin)
            _mainWindowService.NavigateTo(new AdminLayout(_dataService, _mainWindowService, userDTO));
        else
            _mainWindowService.NavigateTo(new UserLayout(_dataService, _mainWindowService, userDTO));
    }

    private async void btnSign_Click(object sender, RoutedEventArgs e) {
        string dni = txbSgDNI.Text.Trim();
        string email = txbUserEmail.Text.Trim();
        string userName = txbSgUserName.Text.Trim();
        string password = pwbSgPassword.Password.Trim();

        Result validation = _dataService.UserService.ValidateUserFields(dni, email, userName, password);
        if (!validation.Success) {
            await _mainWindowService.ShowMsgAsync(validation.Description);
            return;
        }

        //Verificar que DNI no exista (querria decir que ya hay un usuario registrado)
        if (await _dataService.UserService.ExistsUserAsync(txbSgDNI.Text)) {
            await _mainWindowService.ShowMsgAsync("A user with that DNI is already registered");
            return;
        }

        //Confirmar que usuario no quiera agregar imagen de perfil
        if (_userImgPath is null) {
            MessageDialogResult dialogResult = await _mainWindowService.ShowMsgAsync(
                "Do you want to create your user without image? (Default image will be assigned)", MessageDialogStyle.AffirmativeAndNegative
            );

            if (dialogResult == MessageDialogResult.Negative) return;
        }

        //Crear usuario y mostrar resultado
        Result result = await _dataService.UserService.CreateUserAsync(dni, email, userName, password, _userImgPath);
        await _mainWindowService.ShowMsgAsync(result.Description);
        if (result.Success)
            PrepareLoginAfterSignUp(dni, password);
    }

    private void btnRecoverPassword_Click(object sender, RoutedEventArgs e)
        => _mainWindowService.NavigateTo(new RecoverPasswordPage(_dataService, _mainWindowService));

    private void btnImgDialog_Click(object sender, RoutedEventArgs e) {
        var opfDialog = new OpenFileDialog() { Filter = "Images|*.png;*.jpg;*.jpeg" };

        if (opfDialog.ShowDialog() == true) _userImgPath = opfDialog.FileName;
    }

    private async void btnExit_Click(object sender, RoutedEventArgs e) {
        var result = await _mainWindowService.ShowMsgAsync("Are you sure do you want to exit?", MessageDialogStyle.AffirmativeAndNegative);

        if (result == MessageDialogResult.Affirmative)
            _mainWindowService.CloseWindow();
    }

    private async void txbDNI_PreviewTextInput(object sender, TextCompositionEventArgs e) {
        //Bloquear teclado e indicar que no se permiten letras en el DNI
        if (!char.IsDigit(e.Text, 0)) {
            e.Handled = true;
            await _mainWindowService.GenerateFloatToolTipAsync((TextBox)sender);
        }
    }
    //-------------------------inner methods-------------------------
    private void PrepareLoginAfterSignUp(string dni, string password) {
        txbSgDNI.Text = string.Empty;
        txbSgUserName.Text = string.Empty;
        txbUserEmail.Text = string.Empty;
        pwbSgPassword.Password = string.Empty;
        txbLgDNI.Text = dni;
        pwbLgPassword.Password = password;
    }

    private Result ValidLogin() {
        var errors = new StringBuilder();
        DniPasswordValidation(txbLgDNI.Text.Trim(), pwbLgPassword.Password.Trim(), errors);

        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }

    private void DniPasswordValidation(string dni, string password, StringBuilder errors) {
        ValidationHelper.ValidateLength(dni, ValidationConsts.MinDniLength, ValidationConsts.MaxDniLength, "DNI", errors);
        ValidationHelper.ValidateLength(password, ValidationConsts.MinPasswordLength, ValidationConsts.MaxPasswordLength, "Password", errors);
    }
}
