using EventsProject.Domain.Abstractions.Repositories;
using EventsProject.Domain.Abstractions.Services;
using EventsProject.Domain.Builders;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EventsProject.Application.Services;

public class EventService : IEventService {
    //------------------------INITIALIZATION------------------------
    private readonly IRepository<EventInfo> _eventInfoRepository;
    private readonly IImgConvert _imgConverter;

    public EventService(IRepository<EventInfo> eventInfoRepository, IImgConvert imgConverter) {
        _eventInfoRepository = eventInfoRepository;
        _imgConverter = imgConverter;
    }
    //------------------------METHODS------------------------
    public async Task<List<EventInfo>> GetEventsAsync() => await _eventInfoRepository.GetAll().ToListAsync();
    public async Task<List<EventInfo>> GetEventsCreatedByIdAsync(int userId)
        => await _eventInfoRepository.GetAll().Where(ev => ev.CreatedById == userId).ToListAsync();
    public async Task<List<EventInfo>> GetEventsWithCartegoriesAsync() 
        => await _eventInfoRepository.GetAll().Include(e => e.Cat).ToListAsync();
    public async Task<Result> CreateEventAsync(
        string title, int capacity, int catId, string artist, string eventDescription,
        DateTime initialTime, DateTime endTime,
        string eventPlace, string eventCity,
        int userId, bool isAdmin, string? eventImgPath = null
    ) {
        //Metodo para convertir de path a binario con su comprobacion
        byte[]? imgBin = null;
        if (eventImgPath != null) {
            //Valida formato, tamaño y que archivo exista
            Result validation = _imgConverter.ValidImgPath(eventImgPath);
            if (!validation.Success)
                return validation;

            //En caso de tener cumplir todos los requisitos los transformamos a binario
            imgBin = await _imgConverter.ImgPathToBinAsync(eventImgPath);
        }

        //Creacion del objeto mediante su clase Builder
        EventInfo newEvent = new EventBuilder()
            .WithGeneralInfo(title, capacity, catId, artist, eventDescription)
            .WithTimeInfo(initialTime, endTime)
            .WithSpotInfo(eventCity, eventPlace)
            .WithAddtionalInfo(userId, isAdmin)
            .WithEventImg(imgBin)
            .Build();

        //Comprobar que el sitio este disponible
        if (!await IsSpotAvailableAsync(newEvent.EventCity, newEvent.EventPlace, newEvent.InitialTime, newEvent.EndTime))
            return Result.Fail("Schedule and spot are not available. Try with another time");

        return await _eventInfoRepository.InsertAsync(newEvent);
    }

    public async Task<Result> UpdateEventAsync(EventInfo eventInfo) => await _eventInfoRepository.UpdateAsync(eventInfo);

    public Result ValidateEventFields(
        bool editValidation, string? title, int? capacity,
        int? catId, string? artist, string? eventDescription,
        DateTime? initialTime, DateTime? endTime,
        string? eventPlace, string? eventCity, int? stateId = null
    )
    {
        //String para ir acumulando errores
        var errors = new StringBuilder();

        //Validacion GeneralInfo
        ValidationHelper.ValidateLength(title, ValidationConsts.MinEvTitleLength, ValidationConsts.MaxEvTitleLength, "Title", errors);
        ValidationHelper.ValidateLength(eventDescription, ValidationConsts.MinEvDescriptionLength, ValidationConsts.MaxEvDescriptionLength, "Description", errors);
        ValidationHelper.ValidateLength(artist, ValidationConsts.MinEvArtistsLength, ValidationConsts.MaxEvArtistsLength, "Artist/Artists", errors);
        ValidationHelper.ValidateRange(capacity, ValidationConsts.MinEvCapacity, ValidationConsts.MaxEvCapacity, "Capacity", errors);
        ValidationHelper.ValidateNull(catId, "Category", errors);

        //Comprobacion para edicion de eventos, ya que al crear los state se definen en el builder por default
        if (editValidation)
            ValidationHelper.ValidateNull(stateId, "EventState", errors);

        //Validacion DateTimeInfo
        ValidationHelper.ValidateDates(initialTime, endTime, errors);

        //Validacion SpotInfo
        ValidationHelper.ValidateLength(eventCity, ValidationConsts.MinEvCityLength, ValidationConsts.MaxEvCityLength, "City", errors);
        ValidationHelper.ValidateLength(eventPlace, ValidationConsts.MinEvPlaceLength, ValidationConsts.MaxEvPlaceLength, "Place", errors);

        //Devolver el result con el mensaje y resultado correpondiente
        if (errors.Length == 0) return Result.Ok("Correct inputs");
        return Result.Fail("Incorrect Inputs:\n" + errors.ToString());
    }

    public async Task<bool> IsSpotAvailableAsync(string eventCity, string eventPlace, DateTime initTime, DateTime endTime) {
        //Comprobar si existen registros en que la ciudad, lugar sean iguales, estado de evento sea activo o este en tramite (pendiente)
        // y la fecha de incio y fin del evento este en el rango de duracion de otro ya programado
        int matches = await _eventInfoRepository.GetAll()
            .Where(ei => ei.EventCity == eventCity.ToUpper() && ei.EventPlace == eventPlace.ToUpper()
                && (ei.EventStateId == (int)EnumEventState.Active || ei.EventStateId == (int)EnumEventState.Pending)
                && initTime < ei.EndTime && endTime > ei.InitialTime)
            .CountAsync();

        return matches < 1;
    }

    public async Task<Result> ChangeEventStateAsync(EventInfo originalEvent, int newEventStateId) {
        originalEvent.EventStateId = newEventStateId;
        return await _eventInfoRepository.UpdateAsync(originalEvent);
    }

    public async Task<Result> UpdatePastEventsAsync() {
        try {
            //Actualizar eventos que ya se hayam finalizado (fecha y hora menor o igual a la actual)
            var pastEvents = await _eventInfoRepository.GetAll()
                                .Where(ei => ei.EndTime <= DateTime.Now && ei.EventStateId != (int)EnumEventState.Finished)
                                .ToListAsync();

            if (pastEvents.Count == 0) return Result.Ok("Events State are up to date");

            var resultMsgs = new StringBuilder("Events updated:\n");
            foreach (var item in pastEvents) {
                item.EventStateId = (int)EnumEventState.Finished;
                Result result = await _eventInfoRepository.UpdateAsync(item);
                resultMsgs.AppendLine(result.Description);
            }
            return Result.Ok(resultMsgs.ToString());
        }
        catch (Exception ex) { return Result.Fail("There has been an error updating events state", ex.Message); }
    }
}
