using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class UserRegisteredEvent : IDomainEvent
{
    public User User { get; }
    public DateTime OccurredAt { get; }

    public UserRegisteredEvent(User user)
    {
        User = user;
        OccurredAt = DateTime.UtcNow;
    }
}
