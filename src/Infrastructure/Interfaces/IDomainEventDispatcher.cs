using Infrastructure.Entities;

namespace Infrastructure.Interfaces;
public interface IDomainEventDispatcher
{
	Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
}
