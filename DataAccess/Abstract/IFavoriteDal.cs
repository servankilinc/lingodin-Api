using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface IFavoriteDal : IRepository<Favorite>, IRepositoryAsync<Favorite>
{
}