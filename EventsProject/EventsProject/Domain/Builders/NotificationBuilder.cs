using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Builders;

public class NotificationBuilder {
    //-------------------------INITIALIZATION-------------------------
    private NotificationInfo _notification;
    public NotificationBuilder() => _notification = new NotificationInfo();

    //-------------------------METHODS-------------------------
    public NotificationInfo Build() => _notification;

    //-------------------------propsMeths-------------------------
    public NotificationBuilder WithMessageAndDate(string message, DateTime? datetime = null) {
        _notification.NotMessage = message;
        _notification.CreatedAt = datetime ?? DateTime.Now;

        return this;
    }

    public NotificationBuilder WithState(EnumNotificationState NotState) {
        _notification.NotStateId = (int)NotState;

        return this;
    }

    public NotificationBuilder WithUser(int userId) {
        _notification.UserId = userId;

        return this;
    }
}
