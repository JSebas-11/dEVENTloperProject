using EventsProject.Domain.Common;
using EventsProject.Domain.Models;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  gestionar los datos y la logica de negocio de las notificaciones
public interface INotificationService {
    public Task<List<NotificationInfo>> GetNoReadNotificationsByUserAsync(int userId);
    public Task<Result> CreateNotificationAsync(EnumNotificationType notType, int userId, string eventTitle, int? amount = null);
    public Task<Result> ChangeNotificationStateAsync(NotificationInfo notification, EnumNotificationState notificationState);
}