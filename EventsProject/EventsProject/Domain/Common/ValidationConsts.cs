namespace EventsProject.Domain.Common;

//Tamaños minimos y maximos para los campos definidos en la DB y otras
//limitaciones de reglas de negocio
public static class ValidationConsts {
    
    //UserAccount
    public const int MinDniLength = 10;
    public const int MaxDniLength = 12;
    public const int MinUserNameLength = 5;
    public const int MaxUserNameLength = 32;
    public const int MinPasswordLength = 5;
    public const int MaxPasswordLength = 20;
    public const int MinEmailLength = 8;
    public const int MaxEmailLength = 128;
    public const string EmailPattern = @"^[\w\.-]+@[\w-]+\.[a-zA-Z]{2,}$";
    public const string PasswordPattern = @"^(?=\w*\d)(?=\w*[A-Z])(?=\w*[a-z])\S{5,20}$";
    
    //UserEvent
    public const int MaxEventsByUser = 5;

    //Category
    public const int MinCatNameLength = 3;
    public const int MaxCatNameLength = 16;
    public const int MinCatDescriptionLength = 5;
    public const int MaxCatDescriptionLength = 64;
    
    //EventState
    public const int MinStateNameLength = 3;
    public const int MaxStateNameLength = 16;
    
    //EventInfo
    public const int MinEvTitleLength = 5;
    public const int MaxEvTitleLength = 32;
    public const int MinEvPlaceLength = 3;
    public const int MaxEvPlaceLength = 32;
    public const int MinEvCityLength = 3;
    public const int MaxEvCityLength = 32;
    public const int MinEvArtistsLength = 3;
    public const int MaxEvArtistsLength = 128;
    public const int MinEvDescriptionLength = 5;
    public const int MaxEvDescriptionLength = 256;
    public const int MinEvCapacity = 0;
    public const int MaxEvCapacity = 1000000;

    //NotificationState
    public const int MinNotStateNameLength = 3;
    public const int MaxNotStateNameLength = 16;
    
    //NotificationInfo
    public const int MinNotMsgLength = 3;
    public const int MaxNotMsgLength = 128;
}
