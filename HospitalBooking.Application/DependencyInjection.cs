namespace HospitalBooking.Application;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using HospitalBooking.Application.Abstractions.CQRS;
using FluentValidation;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // Register Validators using FluentValidation dependency injection
         services.AddValidatorsFromAssembly(assembly); // Will add this back when I add the package explicitly to App layer or just keep it manual if simple

        // Scan and register all Command/Query Handlers
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && 
                (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                 i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                 i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            ));

        foreach (var type in handlerTypes)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    (interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                     interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     interfaceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }

        return services;
    }
}
