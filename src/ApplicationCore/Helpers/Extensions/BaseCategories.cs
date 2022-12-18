using ApplicationCore.DataAccess;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Helpers;
public static class BaseCategoriesHelpers
{
	public static IEnumerable<TEntity> AllRootItems<TEntity>(this DbSet<TEntity> categoryDbSet)
		 where TEntity : BaseCategory
	{
		return categoryDbSet.Where(item => !item.Removed && item.ParentId == 0);
	}
	public static IEnumerable<TEntity> AllSubItems<TEntity>(this DbSet<TEntity> categoryDbSet)
		 where TEntity : BaseCategory
	{
		return categoryDbSet.Where(item => !item.Removed && item.ParentId > 0);
	}
	public static void SyncSubItems<TEntity>(this IDefaultRepository<TEntity> repository, ICollection<TEntity> existingList, ICollection<TEntity> latestList)
		 where TEntity : BaseCategory
	{
		if (latestList.IsNullOrEmpty()) latestList = new List<TEntity>();

		foreach (var existingItem in existingList)
		{
			if (!latestList.Any(item => item.Id == existingItem.Id))
			{
				existingItem.ParentId = 0;
				existingItem.Removed = true;
			}
		}

		foreach (var latestItem in latestList)
		{
			var existingItem = existingList.Where(item => item.Id == latestItem.Id).FirstOrDefault();

			if (existingItem != null) repository.DbContext.Entry(existingItem).CurrentValues.SetValues(latestItem);
			else repository.DbSet.Add(latestItem);

		}

		repository.DbContext.SaveChanges();
	}

	public static List<int>? ResolveSelectedIds(this DbSet<BaseCategory> categoryDbSet, int[] selectedIds)
	{
		if (selectedIds.IsNullOrEmpty()) return null;

		int lastId = selectedIds[selectedIds.Length - 1];
		int parentId = 0;
		if (selectedIds.Length == 1)
		{

		}
		else
		{
			parentId = selectedIds[selectedIds.Length - 2];
		}

		if (lastId > 0) return new List<int> { lastId };

		if (parentId == 0) return null;

		return categoryDbSet.Where(item => !item.Removed && item.ParentId == parentId).Select(item => item.Id).ToList();

	}

}
