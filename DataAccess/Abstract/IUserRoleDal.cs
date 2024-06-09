using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface IUserRoleDal : IRepository<UserRoles>, IRepositoryAsync<UserRoles>
{
}