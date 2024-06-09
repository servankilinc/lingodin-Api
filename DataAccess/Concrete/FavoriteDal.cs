using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class FavoriteDal : EFRepositoryBase<Favorite, BaseDBContext>, IFavoriteDal
{
    public FavoriteDal(BaseDBContext context) : base(context)
    {
    }
}