using Ardalis.Specification.EntityFrameworkCore;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.DataAccess;
public interface IDefaultRepository<T> : IRepository<T>, IReadRepository<T> where T : class, IAggregateRoot
{
	DefaultContext DbContext { get; }
	DbSet<T> DbSet { get; }
}
public class DefaultRepository<T> : RepositoryBase<T>, IDefaultRepository<T> where T : class, IAggregateRoot
{
	protected readonly DefaultContext _context;
	public DefaultRepository(DefaultContext dbContext) : base(dbContext)
	{
		_context = dbContext;
	}
	public DefaultContext DbContext => _context;
	public DbSet<T> DbSet => _context.Set<T>();

}
