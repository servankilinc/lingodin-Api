using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface IOTPDal : IRepository<OTP>, IRepositoryAsync<OTP>
{
}