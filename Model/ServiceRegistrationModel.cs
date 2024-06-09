using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Model;

public static class ServiceRegistrationModel
{
    public static IServiceCollection AddModelServices(this IServiceCollection service)
    {

        // ****** FluentValidation Injection ******
        service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return service;
    }
}