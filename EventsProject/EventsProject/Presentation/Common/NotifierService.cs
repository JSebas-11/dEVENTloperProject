using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Windows;

namespace EventsProject.Presentation.Common;

public class NotifierService : IDisposable {
    //-------------------------INITIALIZATION-------------------------
    private readonly Notifier _notifier;

    public NotifierService(Window window) {
        _notifier = new(
            cfg => {
                cfg.PositionProvider = new WindowPositionProvider(window, Corner.BottomRight, 10, 10);
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(2), MaximumNotificationCount.FromCount(3));
                cfg.Dispatcher = App.Current.Dispatcher;
            }
        );
    }

    //-------------------------METHODS-------------------------
    public void Show(string msg, EnumNotifierType type) {
        switch (type) {
            case EnumNotifierType.Info:
                _notifier.ShowInformation(msg);
                break;
            case EnumNotifierType.Success:
                _notifier.ShowSuccess(msg);
                break;
            case EnumNotifierType.Warning:
                _notifier.ShowWarning(msg);
                break;
            case EnumNotifierType.Error:
                _notifier.ShowError(msg);
                break;
            default:
                break;
        }
    }

    public void Dispose() => _notifier.Dispose();
}
