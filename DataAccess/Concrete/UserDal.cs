using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class UserDal : EFRepositoryBase<User, BaseDBContext>, IUserDal
{
    public UserDal(BaseDBContext context) : base(context)
    {
    }
}