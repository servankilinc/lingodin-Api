using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface IUserDal : IRepository<User>, IRepositoryAsync<User>
{
}