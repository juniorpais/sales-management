using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
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
                    _logger.LogWarning(
                        "Event publish attempt {Attempt} failed: {Message}. Retrying in {Delay}s.",
                        attempt, exception.Message, delay.TotalSeconds));
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            LogEvent(@event);
            await Task.CompletedTask;
        });
    }

    private void LogEvent<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        switch (@event)
        {
            case SaleCreatedEvent e:
                _logger.LogInformation(
                    "[SaleCreated] SaleId: {SaleId} | SaleNumber: {SaleNumber} | OccurredAt: {OccurredAt}",
                    e.SaleId, e.SaleNumber, e.OccurredAt);
                break;

            case SaleModifiedEvent e:
                _logger.LogInformation(
                    "[SaleModified] SaleId: {SaleId} | OccurredAt: {OccurredAt}",
                    e.SaleId, e.OccurredAt);
                break;

            case SaleCancelledEvent e:
                _logger.LogInformation(
                    "[SaleCancelled] SaleId: {SaleId} | OccurredAt: {OccurredAt}",
                    e.SaleId, e.OccurredAt);
                break;

            case ItemCancelledEvent e:
                _logger.LogInformation(
                    "[ItemCancelled] SaleId: {SaleId} | ProductId: {ProductId} | OccurredAt: {OccurredAt}",
                    e.SaleId, e.ProductId, e.OccurredAt);
                break;

            default:
                _logger.LogInformation(
                    "[DomainEvent] Type: {EventType} | OccurredAt: {OccurredAt}",
                    typeof(TEvent).Name, @event.OccurredAt);
                break;
        }
    }
}
