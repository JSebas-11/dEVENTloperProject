namespace EventsProject.Domain.Common;

//Enums con relacion indirecta a sus relativos en la DB
public enum EnumEventState { Active = 1, Pending, Cancelled, Finished, SoldOut }
public enum EnumNotificationState { Read = 1, UnRead, Removed }

//Enum con las propiedades que podran ser filtradas para el EventFilterOptions
public enum EnumEvenFilterOptions { All, Title, Place, City, Date, State, Category }

//Enum con los tipos de notificaciones para sus diferentes mensajes
public enum EnumNotificationType { Approved, Rejected, Enrollment }