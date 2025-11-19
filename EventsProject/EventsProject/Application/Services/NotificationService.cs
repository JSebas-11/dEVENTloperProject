using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Builders;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventsProject.Application.Services;

public class NotificationService : INotificationService {
    //-------------------------INITIALIZATION-------------------------
    private readonly IRepository<NotificationInfo> _notificationRepository;
    public NotificationService(IRepository<NotificationInfo> notificationRepository) {
        _notificationRepository = notificationRepository;
    }

    //-------------------------METHODS-------------------------
    public async Task<List<NotificationInfo>> GetNoReadNotificationsByUserAsync(int userId)
        => await _notificationRepository.GetAll()
                .Where(not => not.UserId == userId && not.NotStateId == (int)EnumNotificationState.UnRead)
                .OrderByDescending(not => not.CreatedAt)
                .ToListAsync();

    public async Task<Result> CreateNotificationAsync(EnumNotificationType notType, int userId, string eventTitle, int? amount = null) {
        string notMsg = string.Empty;
        switch (notType) {
            case EnumNotificationType.Approved:
                notMsg = $"Event ({eventTitle}) was approved!. Tickets are now available to buy.";
                break;
            case EnumNotificationType.Rejected:
                notMsg = $"Event ({eventTitle}) was unfortunately rejected. You can submit another request anytime.";
                break;
            case EnumNotificationType.Enrollment:
                notMsg = $"You got enroll in ({eventTitle}) - (Tickets: {amount}) at {DateTime.Now}.";
                break;
        }

        NotificationInfo notification = new NotificationBuilder()
                                        .WithMessageAndDate(notMsg)
                                        .WithState(EnumNotificationState.UnRead)
                                        .WithUser(userId)
                                        .Build();

        return await _notificationRepository.InsertAsync(notification);
    }

    public async Task<Result> ChangeNotificationStateAsync(NotificationInfo notification, EnumNotificationState notificationState) {
        notification.NotStateId = (int)notificationState;
        return await _notificationRepository.UpdateAsync(notification);
    }
}
