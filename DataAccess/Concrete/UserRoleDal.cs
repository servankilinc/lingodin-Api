using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class UserRoleDal : EFRepositoryBase<UserRoles, BaseDBContext>, IUserRoleDal
{
    public UserRoleDal(BaseDBContext context) : base(context)
    {
    }
}