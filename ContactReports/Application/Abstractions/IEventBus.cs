namespace ContactReports.Application.Abstractions;

public interface IEventBus
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : class;
    
}