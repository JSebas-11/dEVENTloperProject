using EventsProject.Domain.Common;

namespace EventsProject.Domain.Abstractions.Services;

//Interface que sera inyectada en IDataService como propiedad para
//  gestionar el envio de correos (en un principio solo codigos de verificacion)
//  por medio del IEmailSender
public interface IEmailSenderService {
    Task<Result> SendVerificationCodeAsync(string targetEmail);
}
