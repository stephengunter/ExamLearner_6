using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class ManualsSpecification : Specification<Manual>
{
	public ManualsSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include(item => item.Features);
	}

	public ManualsSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id)
			.Include(item => item.Features);
	}
}

public class ManualsByParentSpecification : Specification<Manual>
{
	public ManualsByParentSpecification(int parentId, bool features = false)
	{
		if (features)
		{
			Query.Where(item => !item.Removed && item.ParentId == parentId)
				.Include(item => item.Features);
		}
		else
		{
			Query.Where(item => !item.Removed && item.ParentId == parentId);
		}
	}
}


public class FeaturesSpecification : Specification<Feature>
{
	public FeaturesSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public FeaturesSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id);
	}
}
