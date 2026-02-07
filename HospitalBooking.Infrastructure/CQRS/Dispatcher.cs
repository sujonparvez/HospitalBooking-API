namespace HospitalBooking.Infrastructure.CQRS;

using HospitalBooking.Application.Abstractions.CQRS;
using Microsoft.Extensions.DependencyInjection;

public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _provider;

    public Dispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var type = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(type);
        var handler = _provider.GetRequiredService(handlerType);
        
        // Use reflection to invoke Handle method
        var method = handlerType.GetMethod("Handle");
        if (method != null)
        {
            await (Task)method.Invoke(handler, new object[] { command, cancellationToken })!;
        }
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var type = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(type, typeof(TResponse));
        var handler = _provider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("Handle");
        if (method != null)
        {
            return await (Task<TResponse>)method.Invoke(handler, new object[] { command, cancellationToken })!;
        }
        throw new InvalidOperationException($"Handler not found for {type.Name}");
    }

    public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var type = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TResponse));
        var handler = _provider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("Handle");
        if (method != null)
        {
            return await (Task<TResponse>)method.Invoke(handler, new object[] { query, cancellationToken })!;
        }
        throw new InvalidOperationException($"Handler not found for {type.Name}");
    }
}
