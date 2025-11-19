using EventsProject.Domain.Models;
using EventsProject.Domain.Common;

namespace EventsProject.Domain.Builders;

class EventBuilder {
    //-------------------------INITIALIZATION-------------------------
    private EventInfo _event;

    public EventBuilder() => _event = new EventInfo();

    //-------------------------METHODS-------------------------
    public EventInfo Build() => _event;

    //-------------------------propsMeths-------------------------
    public EventBuilder WithGeneralInfo(string title, int capacity, int catId, string artists, string description) {
        _event.Title = title.Trim();
        _event.Capacity = capacity;
        _event.CatId = catId;
        _event.Artist = artists.Trim();
        _event.EventDescription = description.Trim();

        return this;
    }

    public EventBuilder WithTimeInfo(DateTime initialTime, DateTime endTime) {
        _event.InitialTime = initialTime;
        _event.EndTime= endTime;
        _event.EventDate = initialTime.Date;
        _event.MinutesDuration = (int)(endTime - initialTime).TotalMinutes;

        return this;
    }

    public EventBuilder WithSpotInfo(string eventCity, string eventPlace) {
        _event.EventCity = eventCity.Trim().ToUpper();
        _event.EventPlace = eventPlace.Trim().ToUpper();

        return this;
    }
    
    public EventBuilder WithAddtionalInfo(int userId, bool isAdmin) {
        _event.CreatedById = userId;
        _event.EventStateId = isAdmin ? (int)EnumEventState.Active : (int)EnumEventState.Pending;

        return this;
    }
    
    public EventBuilder WithAddtionalInfo(int? userId, int? stateId) {
        _event.CreatedById = userId;
        _event.EventStateId = stateId;

        return this;
    }
    
    public EventBuilder WithEventImg(byte[]? eventImg = null) {
        _event.EventImg = eventImg;

        return this;
    }

}
