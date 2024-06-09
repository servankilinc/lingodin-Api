using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface ICategoryDal : IRepository<Category>, IRepositoryAsync<Category>  
{
}