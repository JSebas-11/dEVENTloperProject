using EventsProject.Application.Services;
using EventsProject.Application.Utilities;
using EventsProject.Domain.Abstractions;
using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Abstractions.Utilities;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Infrastructure;
using EventsProject.Infrastructure.Data;
using EventsProject.Infrastructure.Repositories;
using EventsProject.Infrastructure.Utilities;
using EventsProject.Presentation.UI.Containers;
using EventsProject.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;

namespace EventsProject;

public partial class App : System.Windows.Application {
    //------------------------INITIALIZATION------------------------
    private ServiceProvider _provider;

    //------------------------METHODS------------------------
    //------------------------override methods------------------------
    protected override async void OnStartup(StartupEventArgs e) {
        try {
            base.OnStartup(e);

            _provider = DefineDependencies();
            IDataService dataService = _provider.GetRequiredService<IDataService>();
            IAppConfiguration config = _provider.GetRequiredService<IAppConfiguration>();

            //Actualizacion de eventos pasados (fecha y hora anterior a la actual)
            Result resultUpdate = await dataService.EventService.UpdatePastEventsAsync();
            Debug.WriteLine(resultUpdate.Description);

            //Creacion usuario admin default
            var admin = config.GetDefaultAdmin();
            if (!await dataService.UserService.ExistsUserAsync(admin.Dni)) {
                Result result = await dataService.UserService.CreateUserAsync(admin.Dni, admin.UserEmail, admin.UserName, admin.HashPassword, isAdmin: true);
                Debug.WriteLine(result.Description);
            }

            DefineMainWindow(dataService);
        
        }
        catch (Exception ex) {
            MessageBox.Show($"Critical error launching application ({ex.Message})");
            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e){
        await _provider.DisposeAsync();
        base.OnExit(e);
    }

    //------------------------inner methods------------------------
    private void DefineMainWindow(IDataService dataService){
        var mainScreen = new MainWindow(dataService);
        mainScreen.Show();

        MainWindow = mainScreen;
    }
    private ServiceProvider DefineDependencies(){
        var service = new ServiceCollection();

        //Creacion de Configuration (Uso en Services y demas)
        IAppConfiguration config = new AppConfiguration();
        service.AddSingleton(config);

        //Obtener objeto STMP para procesos de emails
        SMTPConfig smtp = config.GetSmtpConfig();

        //Obtener conexion del dispositivo correspondiente y registrar DBContext
        string strConn = config.GetConnection("DefaultConnection");
        service.AddDbContext<EventsProjectContext>(op => op.UseSqlServer(strConn), ServiceLifetime.Scoped);

        //Inyeccion de Repositories (Uso en IDataMetrics y los IServices) 
        service.AddScoped<IReadOnlyRepository<Category>, CategoryRepository>();
        service.AddScoped<IReadOnlyRepository<EventState>, EventStateRepository>();
        service.AddScoped<IRepository<EventInfo>, EventInfoRepository>();
        service.AddScoped<IRepository<NotificationInfo>, NotificationInfoRepository>();
        service.AddScoped<IRepository<UserAccount>, UserAccountRepository>();
        service.AddScoped<IUserEventRepository<UserEvent>, UserEventRepository>();

        //Inyeccion de Utilities (Uso en IServices)
        service.AddSingleton<IHasher, PasswordHasher>();
        service.AddSingleton<IImgConvert, ImgConverter>();
        service.AddSingleton<IEventFilter, EventFilter>();
        service.AddSingleton<IEmailSender>(es => new EmailSender(smtp));

        //Inyeccion de Services (Uso en IDataService)
        service.AddScoped<IDataMetrics, DataMetrics>();
        service.AddScoped<ICategoryService, CategoryService>();
        service.AddScoped<IEventStateService, EventStateService>();
        service.AddScoped<IEventService, EventService>();
        service.AddScoped<IUserAccountService, UserAccountService>();
        service.AddScoped<IUserEventService, UserEventService>();
        service.AddScoped<INotificationService, NotificationService>();
        service.AddScoped<IFilterService, FilterService>();
        service.AddScoped<IEmailSenderService, EmailSenderService>();
        service.AddSingleton<IUtilitiesService, UtilitiesService>();

        //Inyeccion de servicios principales para interactuar en la UI
        service.AddScoped<IDataService, DataService>();

        return service.BuildServiceProvider();
    }
}
