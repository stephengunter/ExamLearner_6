using Infrastructure.Entities;

namespace ApplicationCore.Exceptions;
public class EntityNotFoundException : Exception
{
	public EntityNotFoundException(EntityBase entity) : base($"EntityNotFound. Type: {entity.GetType().Name}  Id: {entity.Id}")
	{

	}
}
