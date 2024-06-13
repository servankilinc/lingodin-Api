﻿using DataAccess.Abstract.RepositoryBase;
using Model.Entities;

namespace DataAccess.Abstract;

public interface IWordDal : IRepository<Word>, IRepositoryAsync<Word>
{
}