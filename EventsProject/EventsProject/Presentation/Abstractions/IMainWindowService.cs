using EventsProject.Presentation.Common;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace EventsProject.Presentation.Abstractions;

//Interface para inyectar a traves de los pages para uso de metodos
//comunes en la UI
public interface IMainWindowService {
    void NavigateTo(Page page);
    void ShowNotification(string msg, EnumNotifierType type = EnumNotifierType.Info);
    Task ShowDialogAsync(CustomDialog dialog);
    Task CloseDialogAsync(CustomDialog dialog);
    Task<MessageDialogResult> ShowMsgAsync(string msg, MessageDialogStyle msgDialogStyle = MessageDialogStyle.Affirmative, string title = "INFORMATION");
    Task GenerateFloatToolTipAsync(TextBox txb);
    void CloseWindow();
}
