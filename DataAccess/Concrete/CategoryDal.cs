using Core.CrossCuttingConcerns;
using Core.DataAccess.RepositoryBase;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Model.Entities;

namespace DataAccess.Concrete;

[DataAccessExceptionHandler]
public class CategoryDal : EFRepositoryBase<Category, BaseDBContext>, ICategoryDal
{
    public CategoryDal(BaseDBContext context) : base(context)
    {
    }
}