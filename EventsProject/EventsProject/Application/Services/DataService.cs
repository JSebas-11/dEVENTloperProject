using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;
using EventsProject.Infrastructure.Data;
using EventsProject.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EventsProject.Application.Services;

public class DataService : IDataService {
    //------------------------INITIALIZATION------------------------
    private readonly EventsProjectContext _context;
    public string StrConn { get; private set; }
    
    //------------------------SERVICES------------------------
    public IDataMetrics DataMetrics { get; }
    public ICategoryService CategoryService { get; }
    public IEventStateService EventStateService { get; }
    public IEventService EventService { get; }
    public IUserAccountService UserService { get; }
    public IUserEventService UserEventService { get; }
    public INotificationService NotificationService { get; }
    public IUtilitiesService UtilitiesService { get; }
    public IImgConvert ConverterService { get; }
    public IFilterService FilterService { get; }
    public IEmailSenderService EmailService { get; }

    public DataService( EventsProjectContext context,
        IDataMetrics dataMetrics,
        ICategoryService categoryService,
        IEventStateService eventStateService,
        IEventService eventService,
        IUserAccountService userService,
        INotificationService notificationService,
        IUserEventService userEventService,
        IUtilitiesService utilitiesService,
        IImgConvert converterService,
        IFilterService filterService,
        IEmailSenderService emailService
    ) {
        _context = context;
        DataMetrics = dataMetrics;
        CategoryService = categoryService;
        EventStateService = eventStateService;
        EventService = eventService;
        UserService = userService;
        UserEventService = userEventService;
        NotificationService = notificationService;
        UtilitiesService = utilitiesService;
        ConverterService = converterService;
        FilterService = filterService;
        EmailService = emailService;
        StrConn = _context.GetStringConnection();
    }

    //------------------------METHODS------------------------
    public async Task<DataTable?> CustomizedQueryAsync(string selectQuery) {
        DataTable tableResult = new DataTable();

        //Crear conexion a la DB con la conexion establecida en el DBContext
        using var conn = new SqlConnection(StrConn);
        try {
            await conn.OpenAsync();
            using var command = new SqlCommand(selectQuery, conn);

            //Ejecutar query y crear tabla de acuerdo al resultado de la query
            using var reader = await command.ExecuteReaderAsync();
            tableResult.Load(reader);

            return tableResult;
        }
        catch (Exception ex) {
            Result.Fail($"There has been an error in: DataService - CustomizedQueryAsync", ex.Message);
            return null;
        }
    }
    
    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}
