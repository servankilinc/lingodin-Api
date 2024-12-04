using Business.Profiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business;

public static class ServiceRegistrationBussiness
{
    public static IServiceCollection AddBussinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ********** AutoMapper **********
        services.AddAutoMapper(typeof(MappingProfiles));

        // ********** RabbitMQ **********
        //ConnectionFactory connectionFactory = new()
        //{ 
        //    Uri = new Uri(configuration["RabbitMq:URL"] ?? throw new Exception("RabbitMQ HostName Could not read!")),
        //    //HostName = configuration["RabbitMq:HostName"] ?? throw new Exception("RabbitMQ HostName Could not read!"),
        //    //Port = Int16.Parse(configuration["RabbitMq:Port"] ?? throw new Exception("RabbitMQ Port Could not read!")),
        //    //UserName = configuration["RabbitMq:UserName"] ?? throw new Exception("RabbitMQ UserName Could not read!"),
        //    //Password = configuration["RabbitMq:Password"] ?? throw new Exception("RabbitMQ Password Could not read!"),
        //    DispatchConsumersAsync = true
        //};
        //services.AddSingleton<ConnectionFactory>(connectionFactory);
        //services.AddSingleton<MailMessageBrokerProducer>();
        //services.AddHostedService<MailSendingBackgroundService>();


        // ********** GoogleJsonWebSignature ValidationSettings **********
        //GoogleJsonWebSignature.ValidationSettings googlValidationsettings = new()
        //{
        //    Audience = new List<string> { configuration["Google:WebClientId"] ?? throw new Exception("Google Validation WebClientId Could not read!") }
        //};
        //services.AddSingleton<GoogleJsonWebSignature.ValidationSettings>(googlValidationsettings);


        //// ********** Facebook App Settings **********
        //FacebookAppSettings facebookAppSettings = new()
        //{
        //    AppId = configuration["Facebook:AppId"] ?? throw new Exception("FacebookAppSettings AppId Could not read!"),
        //    AppSecret = configuration["Facebook:AppSecret"] ?? throw new Exception("FacebookAppSettings AppSecret Could not read!")
        //};
        //services.AddSingleton<FacebookAppSettings>(facebookAppSettings);

        return services;
    }
}