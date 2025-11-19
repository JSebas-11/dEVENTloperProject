using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.Abstractions;
using EventsProject.Presentation.Common;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EventsProject.Presentation.UI.Pages;

public enum ChangeType { UserImg, UserName, UserEmail, Password }

public partial class UserEdit : Page {
    //-------------------------INITIALIZATION-------------------------
    private readonly IDataService _dataService;
    private readonly IMainWindowService _mainWindowService;
    private readonly UserAccount _user;
    private readonly BitmapImage _userImg;
    private string? _newUserImgPath = null;

    public UserEdit(IDataService dataService, IMainWindowService mainWindowService, UserAccount user, BitmapImage userImg) {
        InitializeComponent();
        _dataService = dataService;
        _mainWindowService = mainWindowService;
        _user = user;
        _userImg = userImg;

        Loaded += UserEdit_Loaded;
    }

    //-------------------------METHODS-------------------------
    //-------------------------btnMeths-------------------------
    private void btnChangeImg_Click(object sender, RoutedEventArgs e) => FileExplorerDialog();

    private async void btnConfirmUserName_Click(object sender, RoutedEventArgs e)
        => await ValidateAndUpdateAsync(ChangeType.UserName, "USERNAME");

    private async void btnConfirmUserEmail_Click(object sender, RoutedEventArgs e)
        => await ValidateAndUpdateAsync(ChangeType.UserEmail, "USEREMAIL");

    private async void btnConfirmPassword_Click(object sender, RoutedEventArgs e)
        => await ValidateAndUpdateAsync(ChangeType.Password, "PASSWORD");

    //-------------------------innerMeths-------------------------
    private void UserEdit_Loaded(object sender, RoutedEventArgs e) {
        try {
            gdMainContainer.DataContext = _user;
            imgContainer.Source = _userImg;
        }
        catch (Exception ex) {
            _mainWindowService.ShowNotification($"Error loading User Info:\n{ex.Message}", EnumNotifierType.Error);
        }
    }

    private async void FileExplorerDialog() {
        var opfDialog = new OpenFileDialog() { Filter = "Images|*.png;*.jpg;*.jpeg" };

        if (opfDialog.ShowDialog() == true) {
            _newUserImgPath = opfDialog.FileName;
            await ValidateAndUpdateAsync(ChangeType.UserImg, "UserImage");
        }
    }

    private async Task ValidateAndUpdateAsync(ChangeType changeType, string propertieName) {
        //Metodo generico para converger las operaciones de validacion y actualizacion de cada campos
        //  con el fin de abstraer la logica de los botones
        Result validation = ValidInputs(changeType);

        if (!validation.Success) {
            _mainWindowService.ShowNotification(validation.Description, EnumNotifierType.Warning);
            return;
        }

        Result changeResult = await ApplyChangesAsync(changeType);

        if (!changeResult.Success) {
            _mainWindowService.ShowNotification($"There has been an error updating your {propertieName}:" +
                $"\n{changeResult.Description}", EnumNotifierType.Error);
            return;
        }

        _mainWindowService.ShowNotification($"Your {propertieName} has been updated successfully", EnumNotifierType.Success);
    }

    private async Task<Result> ApplyChangesAsync(ChangeType changeType) {
        //Actualizar propiedad del usuario con elemento del front segun corresponda con el Enum
        switch (changeType) {
            case ChangeType.UserImg:
                _user.UserImg = await _dataService.ConverterService.ImgPathToBinAsync(_newUserImgPath);
                break;

            case ChangeType.UserName:
                _user.UserName = txbUsername.Text.Trim();
                break;

            case ChangeType.UserEmail:
                _user.UserEmail = txbUserEmail.Text.Trim();
                break;

            case ChangeType.Password:
                _user.HashPassword = _dataService.UtilitiesService.HashPassword(pwbPassword.Password.Trim());
                break;

            default:
                break;
        }

        Result updateResult = await _dataService.UserService.UpdateUserAsync(_user);
        if (!updateResult.Success)
            return Result.Fail(updateResult.Description);

        CleanInputs(changeType);
        gdMainContainer.DataContext = null;
        gdMainContainer.DataContext = _user;
        return Result.Ok(updateResult.Description);
    }

    private void CleanInputs(ChangeType changes) {
        switch (changes) {
            case ChangeType.UserImg:
                _newUserImgPath = string.Empty;
                break;

            case ChangeType.UserName:
                txbUsername.Text = string.Empty;
                break;

            case ChangeType.UserEmail:
                txbUserEmail.Text = string.Empty;
                break;

            case ChangeType.Password:
                pwbPassword.Password = string.Empty;
                pwbConfPassword.Password = string.Empty;
                break;

            default:
                break;
        }
    }

    private Result ValidInputs(ChangeType change) {
        //String para ir acumulando errores
        var errors = new StringBuilder();

        switch (change) {
            case ChangeType.UserImg:
                Result validation = _dataService.ConverterService.ValidImgPath(_newUserImgPath);
                if (!validation.Success)
                    errors.AppendLine(validation.Description);
                break;

            case ChangeType.UserName:
                string userName = txbUsername.Text.Trim();
                ValidationHelper.ValidateLength(userName, ValidationConsts.MinUserNameLength, ValidationConsts.MaxUserNameLength, "UserName", errors);
                if (userName == _user.UserName) 
                    errors.AppendLine("- UserName doesn't have any changes");
                break;

            case ChangeType.UserEmail:
                string newEmail = txbUserEmail.Text.Trim();
                ValidationHelper.ValidateEmail(newEmail, errors);
                if (newEmail == _user.UserEmail)
                    errors.AppendLine("- UserEmail doesn't have any changes");
                break;

            case ChangeType.Password:
                string newPassword1 = pwbPassword.Password.Trim();
                string newPassword2 = pwbConfPassword.Password.Trim();
                ValidationHelper.ValidatePassword(newPassword1, errors);
                if (newPassword1 != newPassword2)
                    errors.AppendLine("- Password and its confirmation don't match");
                else if (_dataService.UtilitiesService.VerifyPassword(newPassword1, _user.HashPassword))
                    errors.AppendLine("- Password doesn't have any changes");
                break;

            default:
                break;
        }

        //Devolver el result con el mensaje y resultado correpondiente
        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }
}
