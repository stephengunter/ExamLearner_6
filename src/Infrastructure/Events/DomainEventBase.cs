using MediatR;

namespace Infrastructure.Events;

public abstract class DomainEventBase : INotification
{
	public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
