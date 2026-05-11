using Ambev.DeveloperEvaluation.Common.Events;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Ambev.DeveloperEvaluation.IoC.Services;

public class LogEventPublisher : IEventPublisher
{
    private readonly ILogger<LogEventPublisher> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public LogEventPublisher(ILogger<LogEventPublisher> logger)
    {
        _logger = logger;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, delay, attempt, _) =>
                    _logger.LogWarning("Event publish attempt {Attempt} failed: {Message}. Retrying in {Delay}s.",
                        attempt, exception.Message, delay.TotalSeconds));
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Domain event published: {EventType} at {OccurredAt}",
                typeof(TEvent).Name, @event.OccurredAt);

            await Task.CompletedTask;
        });
    }
}
