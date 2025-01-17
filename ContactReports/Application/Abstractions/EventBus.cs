using MassTransit;

namespace ContactReports.Application.Abstractions;

public sealed class EventBus : IEventBus
{
    private readonly IPublishEndpoint publishEndpoint;

    public EventBus(IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
    }

    public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        return publishEndpoint.Publish(message, cancellationToken);
    }
}