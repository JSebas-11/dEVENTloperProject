using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using EventsProject.Presentation.DTOs;
using EventsProject.Properties;
using EventsProject.Utilities;

namespace EventsProject.Presentation.Mappers;

public static class EventMapper {
    public static async Task<EventDTO> ToDTOAsync(EventInfo eventInfo, IImgConvert imgConverter) {
        return new EventDTO() {
            EventId = eventInfo.EventId,
            Title = eventInfo.Title,
            EventPlace = eventInfo.EventPlace,
            EventCity = eventInfo.EventCity,
            InitialTime = eventInfo.InitialTime,
            EndTime = eventInfo.EndTime,
            Artist = eventInfo.Artist,
            EventDescription = eventInfo.EventDescription,
            Category = eventInfo.Cat?.CatName,
            Capacity = eventInfo.Capacity,
            EventState = (EnumEventState)eventInfo.EventStateId,
            EventImg = eventInfo.EventImg == null ? imgConverter.BinToImg(Resources.defaultEvIMG) 
                    : imgConverter.BinToImg(eventInfo.EventImg)
        };
    }

    public static async Task<List<EventDTO>> ToDTOListAsync(IEnumerable<EventInfo> eventInfoList, IImgConvert imgConverter) {
        List<EventDTO> dtosList = [];
        foreach (EventInfo item in eventInfoList) { dtosList.Add(await ToDTOAsync(item, imgConverter)); }
        return dtosList;
    }
}
