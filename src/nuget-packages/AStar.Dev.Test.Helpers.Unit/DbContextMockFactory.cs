using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NSubstitute;

namespace AStar.Dev.Test.Helpers.Unit;

/// <summary>
/// </summary>
public static class DbContextMockFactory
{
    /// <summary>
    /// </summary>
    /// <param name="backingStore"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static DbSet<T> CreateMockDbSet<T>(IList<T> backingStore) where T : class
    {
        var queryable = backingStore.AsQueryable();

        var mockSet = Substitute.For<DbSet<T>, IQueryable<T>>();
        ((IQueryable<T>)mockSet).Provider.Returns(queryable.Provider);
        ((IQueryable<T>)mockSet).Expression.Returns(queryable.Expression);
        ((IQueryable<T>)mockSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<T>)mockSet).GetEnumerator().Returns(queryable.GetEnumerator());

        mockSet.Add(Arg.Do<T>(item => backingStore.Add(item)));
        mockSet.Remove(Arg.Do<T>(item => backingStore.Remove(item)));

        mockSet.AddAsync(Arg.Any<T>(), Arg.Any<CancellationToken>())
               .Returns(call =>
                        {
                            var entity = call.Arg<T>();
                            backingStore.Add(entity);

                            return new (Task.FromResult((EntityEntry<T>)null!)); // No actual tracking
                        });

        return mockSet;
    }

    /// <summary>
    /// </summary>
    /// <param name="backingStore"></param>
    /// <param name="dbSetSelector"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static TContext CreateMockDbContext<TContext, TEntity>(
        IList<TEntity>                             backingStore,
        Expression<Func<TContext, DbSet<TEntity>>> dbSetSelector
    ) where TContext : DbContext
      where TEntity : class
    {
        var mockSet     = CreateMockDbSet(backingStore);
        var mockContext = Substitute.For<TContext>(new DbContextOptions<TContext>());

        dbSetSelector.Compile().Invoke(mockContext).Returns(mockSet);

        mockContext.SaveChangesAsync(Arg.Any<CancellationToken>())
                   .Returns(Task.FromResult(1));

        return mockContext;
    }
}
