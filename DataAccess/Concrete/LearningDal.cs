using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class LearningDal : EFRepositoryBase<Learned, BaseDBContext>, ILearningDal
{
    public LearningDal(BaseDBContext context) : base(context)
    {
    }
}