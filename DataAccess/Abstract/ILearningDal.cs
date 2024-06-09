using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface ILearningDal : IRepository<Learned>, IRepositoryAsync<Learned>
{
}