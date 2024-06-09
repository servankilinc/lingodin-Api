using AutoMapper;
using Business.Abstract;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using Model.Dtos.CategoryDtos;
using Model.Entities;
using Model.ViewModels;
using Core.Exceptions;
using Core.CrossCuttingConcerns;
using Business.CacheKeys;
using Core.Utils.Caching;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class CategoryService : ICategoryService
{
    private readonly ICategoryDal _categoryDal;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private const string CategoryGroup = "Category-Cache-Group-Key";
    private const string CategoryUserGroup = "Category-User-Cache-Group-Key";
    private const string AllCategoryList = "AllCategoryListCacheKey";
    private const string CategoryInfoById = "CategoryInfoByIdCacheKey";
    private const string AllCategoryListForUser = "AllCategoryListForUserCacheKey";
    public CategoryService(ICategoryDal categoryDal, ICacheService cacheService, IFileService fileService, IMapper mapper)
    {
        _categoryDal = categoryDal;
        _cacheService = cacheService;
        _fileService = fileService;
        _mapper = mapper; 
    }


    [Cache(CategoryInfoById, [CategoryGroup])]
    public async Task<CategoryResponseDto> GetCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));

        Category category = await _categoryDal.GetAsync(filter: c => c.Id == categoryId);
        CategoryResponseDto categoryResponseDto = _mapper.Map<CategoryResponseDto>(category);
        return categoryResponseDto;
    }

    
    [Cache(AllCategoryList, [CategoryGroup])]
    public async Task<ICollection<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var list = await _categoryDal.GetAllAsync();
        var mappedList = list.Select(c => _mapper.Map<CategoryResponseDto>(c)).ToList();
        return mappedList;
    }


    [CacheRemoveGroup([CategoryGroup, CategoryUserGroup])]
    [Validation(typeof(CategoryCreateDto))]
    public async Task<CategoryResponseDto> InsertCategoryAsync(CategoryCreateDto cateogoryCreateDto)
    {
        Category category = _mapper.Map<Category>(cateogoryCreateDto);
        Category insertedCategory = await _categoryDal.AddAsync(category);
        CategoryResponseDto categoryResponseDto = _mapper.Map<CategoryResponseDto>(insertedCategory);
        return categoryResponseDto;
    }


    [CacheRemoveGroup([CategoryGroup, CategoryUserGroup])]
    [Validation(typeof(CategoryUpdateDto))]
    public async Task<CategoryResponseDto> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
    { 
        Category existingCategory = await _categoryDal.GetAsync(filter: c => c.Id == categoryUpdateDto.Id);
        if (existingCategory == null) throw new BusinessException("Data(existing) for update Not Found !");
        _mapper.Map(source: categoryUpdateDto, destination: existingCategory);
        Category updatedCategory = await _categoryDal.UpdateAsync(existingCategory);
        CategoryResponseDto responseDto = _mapper.Map<CategoryResponseDto>(updatedCategory);
        return responseDto;
    }


    [CacheRemoveGroup([CategoryGroup, CategoryUserGroup])]
    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        var categoryToDelete = new Category { Id = categoryId };
        await _categoryDal.DeleteAsync(categoryToDelete);
    }


    [CacheRemoveGroup([CategoryGroup, CategoryUserGroup])]
    public async Task<CategoryResponseDto> UpdateImageAsync(IFormFile file, Guid categoryId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        if (file == null) throw new ArgumentNullException(nameof(file));

        Category existingCategory = await _categoryDal.GetAsync(filter: c => c.Id == categoryId);
        if (existingCategory == null) throw new BusinessException("Data(existing) for update Not Found !");

        string imageUrl = await _fileService.UploadAsync(
            file: file,
            containerName: "images",
            blobDir: "category",
            customFileName: $"img-{categoryId}"
        );
        existingCategory.HasImage = true;
        existingCategory.Image = imageUrl;

        Category updatedCategory = await _categoryDal.UpdateAsync(existingCategory);
        CategoryResponseDto responseDto = _mapper.Map<CategoryResponseDto>(updatedCategory);
        return responseDto;
    }

    [CacheRemoveGroup([CategoryGroup, CategoryUserGroup])]
    public async Task<CategoryResponseDto> DeleteImageAsync(Guid categoryId, string url)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

        string fileName = Path.GetFileName(url);
        string temp = categoryId.ToString();
        if (fileName.Contains(temp) == false ) throw new BusinessException("Url and category did not match!");

        Category existingCategory = await _categoryDal.GetAsync(filter: c => c.Id == categoryId);
        if (existingCategory == null) throw new BusinessException("Data(existing) for update Not Found !");         

        var result = await _fileService.DeleteAsync(containerName: "images", blobFilename: $"category/{fileName}");
        if (result == false) throw new BusinessException("File has not been deleted!");
        existingCategory.HasImage = false;
        existingCategory.Image = string.Empty;
        Category updatedCategory = await _categoryDal.UpdateAsync(existingCategory);
        CategoryResponseDto responseDto = _mapper.Map<CategoryResponseDto>(updatedCategory);
        return responseDto;
    }


    public async Task<CategoryByUserModel> GetCategoryForUserAsync(Guid categoryId, Guid userId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        // -------- cache on before --------
        string _cacheKey = CategoryCacheKeys.CategoryInfoForUser(categoryId, userId);
        var resultCache = _cacheService.GetFromCache(_cacheKey);
        if (resultCache.IsSuccess)
        {
            var data = JsonConvert.DeserializeObject<CategoryByUserModel>(resultCache.Source!);
            if (data != null) return data;
        }
        // -------- cache on before --------

        var category = await _categoryDal.GetAsync(
            filter: c => c.Id == categoryId,
            include: c => c.Include(i => i.Words)!.ThenInclude(x => x.Learneds)!
        );
        var model =  new CategoryByUserModel
        {
            Category = _mapper.Map<CategoryResponseDto>(category),
            TotalWordCount = category.Words == null ? 0 : category.Words.Count,
            LearnedWordCount = category.Words == null ? 0 : category.Words.Sum(w => w.Learneds == null ? 0 : w.Learneds.Count(l => l.UserId == userId))
        };

        // -------- cache on success --------
        string _cacheCategoryUserGroupKey = CategoryCacheKeys.CategoryUserGroupByUserId(userId); // purpose: related user do any learning process dont remove all cached data just this user
        _cacheService.AddToCache(_cacheKey, [CategoryUserGroup, _cacheCategoryUserGroupKey], model);
        // -------- cache on success --------

        return model;
    }


    [Cache(AllCategoryListForUser, [CategoryUserGroup])]
    public async Task<ICollection<CategoryByUserModel>> GetAllCategoriesForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        var categories = await _categoryDal.GetAllAsync(include: c => c.Include(i => i.Words)!.ThenInclude(x => x.Learneds)!);
        var listModel = categories.Select(c => new CategoryByUserModel
        {
            Category = _mapper.Map<CategoryResponseDto>(c),
            TotalWordCount = c.Words == null ? 0 : c.Words.Count,
            LearnedWordCount =  c.Words == null ? 0 : 
                c.Words.Sum(w => w.Learneds == null ? 0 : w.Learneds.Count(l => l.UserId == userId)) // c.Words.Sum(w =>  w.Learneds.Count(l => l.UserId == userId))
        }).ToList();
        return listModel;
    }
}