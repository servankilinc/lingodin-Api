using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;
namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class OTPDal : EFRepositoryBase<OTP, BaseDBContext>, IOTPDal
{
    public OTPDal(BaseDBContext context) : base(context)
    {
    }
}
