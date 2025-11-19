namespace EventsProject.Domain.Common;

public class EventFilterOptions {
    //------------------------PROPERTIES------------------------
    public EnumEvenFilterOptions FilterField { get; set; }
    public string FilterValue { get; set; }

    //------------------------CONSTRUCTOR------------------------
    public EventFilterOptions(EnumEvenFilterOptions filterField, string filterValue) {
        FilterField = filterField;
        FilterValue = filterValue;
    }
}
