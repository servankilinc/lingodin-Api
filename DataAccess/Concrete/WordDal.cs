using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class WordDal : EFRepositoryBase<Word, BaseDBContext>, IWordDal
{
    public WordDal(BaseDBContext context) : base(context)
    {
    }
}