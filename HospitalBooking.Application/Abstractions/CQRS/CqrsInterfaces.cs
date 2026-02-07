namespace HospitalBooking.Application.Abstractions.CQRS;

public interface ICommand { }
public interface ICommand<out TResponse> : ICommand { }

public interface IQuery<out TResponse> { }

public interface ICommandHandler<in TCommand> 
    where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResponse> 
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface IQueryHandler<in TQuery, TResponse> 
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}

public interface IDispatcher
{
    Task Send(ICommand command, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
    Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
