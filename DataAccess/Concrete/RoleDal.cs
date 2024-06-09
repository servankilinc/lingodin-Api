using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class RoleDal : EFRepositoryBase<Role, BaseDBContext>, IRoleDal
{
    public RoleDal(BaseDBContext context) : base(context)
    {
    }
}