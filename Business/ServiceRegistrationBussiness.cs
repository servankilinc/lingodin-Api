using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Business.BackgroundServices;
using Business.MessageBroker;
using Business.Profiles;
using Core.Utils.Auth;
using Core.Utils.Azure;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Business;

public static class ServiceRegistrationBussiness
{
    public static IServiceCollection AddBussinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ******** AutoMapper **********
        services.AddAutoMapper(typeof(MappingProfiles));


        // ******** Azure Services Settings **********
        AzureSettings azureSettings = new()
        {
            ConnectionStringStorageService = configuration["Azure:ConnectionStringStorageService"] ?? throw new Exception("Azure File Service Could not read access informations!"),
            ConnectionStringMailService = configuration["Azure:ConnectionStringMailService"] ?? throw new Exception("Azure Mail Service Could not read access connection informations!"),
            DefaultMailSenderAddress = configuration["Azure:DefaultMailSenderAddress"] ?? throw new Exception("Azure Default Mail Sender Address Could not read!"),
            CdnDomain = configuration["Azure:CdnDomain"] ?? throw new Exception("Azure CDN Domain Could not read!")
        };
        services.AddSingleton<AzureSettings>(azureSettings);


        // ******** Azure Blob Client **********  
        services.AddSingleton<BlobServiceClient>(new BlobServiceClient(azureSettings.ConnectionStringStorageService));


        // ******** Azure Email Client ********** 
        services.AddSingleton<EmailClient>(new EmailClient(azureSettings.ConnectionStringMailService));


        // ******** RabbitMQ **********
        ConnectionFactory connectionFactory = new()
        { 
            Uri = new Uri(configuration["RabbitMq:URL"] ?? throw new Exception("RabbitMQ HostName Could not read!")),
            //HostName = configuration["RabbitMq:HostName"] ?? throw new Exception("RabbitMQ HostName Could not read!"),
            //Port = Int16.Parse(configuration["RabbitMq:Port"] ?? throw new Exception("RabbitMQ Port Could not read!")),
            //UserName = configuration["RabbitMq:UserName"] ?? throw new Exception("RabbitMQ UserName Could not read!"),
            //Password = configuration["RabbitMq:Password"] ?? throw new Exception("RabbitMQ Password Could not read!"),
            DispatchConsumersAsync = true
        };
        services.AddSingleton<ConnectionFactory>(connectionFactory);
        services.AddSingleton<MailMessageBrokerProducer>();
        services.AddHostedService<MailSendingBackgroundService>();


        // ******** GoogleJsonWebSignature ValidationSettings **********
        GoogleJsonWebSignature.ValidationSettings googlValidationsettings = new()
        {
            Audience = new List<string> { configuration["Google:WebClientId"] ?? throw new Exception("Google Validation WebClientId Could not read!") }
        };
        services.AddSingleton<GoogleJsonWebSignature.ValidationSettings>(googlValidationsettings);


        // ******** Facebook App Settings **********
        FacebookAppSettings facebookAppSettings = new()
        {
            AppId = configuration["Facebook:AppId"] ?? throw new Exception("FacebookAppSettings AppId Could not read!"),
            AppSecret = configuration["Facebook:AppSecret"] ?? throw new Exception("FacebookAppSettings AppSecret Could not read!")
        };
        services.AddSingleton<FacebookAppSettings>(facebookAppSettings);

        return services;
    }
}