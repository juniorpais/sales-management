namespace Ambev.DeveloperEvaluation.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
