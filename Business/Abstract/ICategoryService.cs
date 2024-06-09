using Microsoft.AspNetCore.Http;
using Model.Dtos.CategoryDtos;
using Model.ViewModels;

namespace Business.Abstract;

public interface ICategoryService
{
    // 1) Related Entity Methods
    Task<CategoryResponseDto> GetCategoryAsync(Guid categoryId);
    Task<ICollection<CategoryResponseDto>> GetAllCategoriesAsync();
    Task<CategoryResponseDto> InsertCategoryAsync(CategoryCreateDto cateogoryCreateDto);
    Task<CategoryResponseDto> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
    Task DeleteCategoryAsync(Guid categoryId);
    Task<CategoryResponseDto> UpdateImageAsync(IFormFile file, Guid categoryId);
    Task<CategoryResponseDto> DeleteImageAsync(Guid categoryId, string url);

    // 2) Interaction Methods
    Task<CategoryByUserModel> GetCategoryForUserAsync(Guid categoryId, Guid userId);
    Task<ICollection<CategoryByUserModel>> GetAllCategoriesForUserAsync(Guid userId);
}