using Core.Model;
using Core.Utils.DynamicQuery;
using Core.Utils.Pagination;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DataAccess.Abstract.RepositoryBase;

public interface IRepository<TEntity> where TEntity : IEntity
{
    TEntity Get(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    TEntity Add(TEntity entity);    
    TEntity Update(TEntity entity);    
    void Delete(TEntity entity);
    bool IsExist(Expression<Func<TEntity, bool>> filter);


    ICollection<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = true
    );

    Paginate<TEntity> GetPaginatedList(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = default,
        int size = default,
        bool enableTracking = true
    );

    ICollection<TEntity> GetAllByDynamic(
        DynamicQuery dynamicQuery,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool enableTracking = true
    );

    Paginate<TEntity> GetPaginatedListByDynamic(
        DynamicQuery dynamicQuery,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = default,
        int size = default,
        bool enableTracking = true
    );
}