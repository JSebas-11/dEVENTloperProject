using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.UI.Forms;
using EventsProject.Presentation.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventsProject.Presentation.UI.Pages;

public partial class RecoverPasswordPage : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private int? _code = null;
    
    public RecoverPasswordPage(IDataService dataService, IMainWindowService mainWindowService) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        CleanTargetEmail();
    }

    //-------------------------METHODS-------------------------
    //-------------------------btns methods-------------------------
    private void btnBack_Click(object sender, RoutedEventArgs e) => _mainWindowService.NavigateTo(new LoginPage(_dataService, _mainWindowService));
    private async void btnConfirmDni_Click(object sender, RoutedEventArgs e) {
        //Reiniciar email label
        CleanTargetEmail();
        string dni = txbDNI.Text.Trim();

        Result resultValidation = VerifyDni(dni);
        if (!resultValidation.Success) {
            _mainWindowService.ShowNotification($"Invalid DNI\n{resultValidation.Description}", EnumNotifierType.Warning);
            return;
        }

        //Verificacion de email (null si usuario no existe)
        string? email = await _dataService.UserService.GetUserEmailAsync(dni);
        if (email is null) {
            _mainWindowService.ShowNotification($"Email not found\nUser with dni ({dni}) does not exist");
            return;
        }

        txbTargetEmail.Text = email;
        _code = null;
        txbVerificationCode.Clear();
    }

    private async void btnSendCode_Click(object sender, RoutedEventArgs e) {
        //Verificaciones para garantizar que el codigo no haya sido enviado y el email este definido
        if (_code is not null) {
            _mainWindowService.ShowNotification("Code Sent\nCode was already sent");
            return;
        }

        if (String.Equals(txbTargetEmail.Text, "Undefined")) {
            _mainWindowService.ShowNotification("Email not defined\nType your DNI to continue", EnumNotifierType.Warning);
            return;
        }

        //Desactivar botones durante el proceso durante el proceso
        loadingOverlay.Visibility = Visibility.Visible;
        EnableButtons(false);
        //Envio correo de correo (en caso de exito retorna el codigo enviado como string)
        Result emailResult = await _dataService.EmailService.SendVerificationCodeAsync(txbTargetEmail.Text);

        if (emailResult.Success) {
            _code = int.Parse(emailResult.Description);
            _mainWindowService.ShowNotification("Email Sent\nVerification code was sent successfully, take a look at your email", EnumNotifierType.Success);
        }
        else
            _mainWindowService.ShowNotification($"Email Error\n{emailResult.Description}", EnumNotifierType.Warning);

        EnableButtons(true);
        loadingOverlay.Visibility = Visibility.Collapsed;
    }

    private async void btnConfirmCode_Click(object sender, RoutedEventArgs e) {
        string codeStr = txbVerificationCode.Text.Trim();
        Result codeValidation = VerifyCode(codeStr);

        //Comprobaciones correspondientes con el codigo
        if (!codeValidation.Success) {
            _mainWindowService.ShowNotification($"Invalid Code\n{codeValidation.Description}", EnumNotifierType.Warning);
            return;
        }

        int code = int.Parse(codeStr);
        if (code != _code) {
            _mainWindowService.ShowNotification("Invalid Code\nCode typed does not coincide with code sent to your email", EnumNotifierType.Warning);
            return;
        }

        string dni = txbDNI.Text.Trim();
        UserAccount? user = await _dataService.UserService.GetUserAsync(dni);
        if (user is null) {
            _mainWindowService.ShowNotification($"Critical error\nThere has been an error getting User (dni-{dni}). It does not exist", EnumNotifierType.Error);
            return;
        }

        await _mainWindowService.ShowDialogAsync(new RestorePasswordForm(_dataService, _mainWindowService, user));
    }

    private async void txbNumber_PreviewTextInput(object sender, TextCompositionEventArgs e) {
        //Bloquear teclado e indicar que no se permiten letras en el DNI
        if (!char.IsDigit(e.Text, 0)) {
            e.Handled = true;
            await _mainWindowService.GenerateFloatToolTipAsync((TextBox)sender);
        }
    }

    //------------------------innerMeths------------------------
    private void EnableButtons(bool enable) {
        btnBack.IsEnabled = enable;
        btnConfirmCode.IsEnabled = enable;
        btnConfirmDni.IsEnabled = enable;
        btnSendCode.IsEnabled = enable;
    }
    private void CleanTargetEmail() => txbTargetEmail.Text = "Undefined";
    private Result VerifyDni(string dni) {
        var errors = new StringBuilder();

        ValidationHelper.ValidateNull(dni, "DNI", errors);
        ValidationHelper.ValidateLength(dni, ValidationConsts.MinDniLength, ValidationConsts.MaxDniLength, "DNI", errors);

        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }

    private Result VerifyCode(string code) {
        var errors = new StringBuilder();

        if (_code is null)
            errors.AppendLine("- Code has not been sent yet");
        ValidationHelper.ValidateNull(code, "Code", errors);
        ValidationHelper.ValidateLength(code, 6, 6, "Code", errors);

        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }

}
