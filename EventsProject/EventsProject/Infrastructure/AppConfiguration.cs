using EventsProject.Domain.Abstractions;
using EventsProject.Domain.Common;
using EventsProject.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace EventsProject.Infrastructure;

public class AppConfiguration : IAppConfiguration {
    //------------------------INITIALIZATION------------------------
    private IConfigurationRoot _configuration;
    public AppConfiguration() {
       _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    //------------------------METHODS------------------------
    public string GetConnection(string connName) => _configuration.GetConnectionString(connName);
    public UserAccount GetDefaultAdmin() {
        IConfiguration adminData = _configuration.GetSection("DefaultAdmin");
        
        return new UserAccount() {
            Dni = adminData["Dni"] ?? string.Empty,
            UserEmail = adminData["Email"] ?? string.Empty,
            //No esta hasheada solo sera una clase transportadora
            HashPassword = adminData["Password"] ?? string.Empty,
            UserName = adminData["UserName"] ?? string.Empty
        };
    }
    public SMTPConfig GetSmtpConfig() {
        IConfiguration smtpData = _configuration.GetSection("SMTPConfig");

        int port = String.IsNullOrWhiteSpace(smtpData["Port"]) ? 587 : int.Parse(smtpData["Port"]);
        bool ssl = String.IsNullOrWhiteSpace(smtpData["EnableSsl"]) ? true : bool.Parse(smtpData["EnableSsl"]);

        return new SMTPConfig(smtpData["Email"], smtpData["Password"], smtpData["Host"], port, ssl);
    }
}
