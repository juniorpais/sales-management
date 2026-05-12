using Ambev.DeveloperEvaluation.Common.Events;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.IoC.Services;

public class RebusEventPublisher : IEventPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<RebusEventPublisher> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RebusEventPublisher(IBus bus, ILogger<RebusEventPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, delay, attempt, _) =>
                    _logger.LogWarning(
                        "Event publish attempt {Attempt} failed: {Message}. Retrying in {Delay}s.",
                        attempt, exception.Message, delay.TotalSeconds));
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _bus.Publish(@event);
            });

            _logger.LogInformation(
                "Event {EventType} published successfully at {OccurredAt}",
                @event.GetType().Name, @event.OccurredAt);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to publish event {EventType} after all retry attempts. Event: {@Event}",
                @event.GetType().Name, @event);
        }
    }
}
