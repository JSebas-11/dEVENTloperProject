namespace EventsProject.Domain.Common;

//Clase para tener informacion de los resultados de las operaciones
public class Result {
    //------------------------INITIALIZATION------------------------
    public bool Success { get; private set; }
    public string Description { get; private set; }
    public string? ExceptionMsg { get; private set; }

    private Result(bool success, string description, string? exceptionMsg = null) {
        Success = success;
        Description = description;
        ExceptionMsg = exceptionMsg;
    }

    //------------------------STATIC METHODS------------------------
    public static Result Ok(string description) => new Result(true, description);
    public static Result Fail(string description, string? exceptionMesg = null) => new Result(false, description, exceptionMesg);
}
