using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Events;
using Infrastructure.Interfaces;

namespace Infrastructure.Entities;
public abstract class EntityBase : IAggregateRoot
{
	public int Id { get; set; }

	private List<DomainEventBase> _domainEvents = new();
	[NotMapped]
	public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

	protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
	internal void ClearDomainEvents() => _domainEvents.Clear();
}
