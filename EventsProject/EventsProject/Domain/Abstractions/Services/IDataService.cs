using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Utilities;
using System.Data;

namespace EventsProject.Domain.Abstractions.Services;

//Declaracion de interface IDataService que sera inyectada en MainWindow para
//  gestionar los datos y toda la logica de negocio a traves de los servicios
public interface IDataService : IAsyncDisposable {
    //---------------------PROPERTIES---------------------
    //---------------------services---------------------
    public ICategoryService CategoryService { get; }
    public IEventStateService EventStateService { get; }
    public IEventService EventService { get; }
    public IUserAccountService UserService { get; }
    public IUserEventService UserEventService { get; }
    public INotificationService NotificationService { get; }

    //---------------------utilities---------------------
    public IDataMetrics DataMetrics { get; }
    public IUtilitiesService UtilitiesService { get; }
    public IImgConvert ConverterService { get; }
    public IFilterService FilterService { get; }
    public IEmailSenderService EmailService { get; }

    //---------------------METHODS---------------------
    public Task<DataTable?> CustomizedQueryAsync(string selectQuery);
    new ValueTask DisposeAsync();
}
