using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.CategoryDtos;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService) => _categoryService = categoryService;
   

    [Authorize]
    [HttpGet("Get")]
    public async Task<IActionResult> Get([FromQuery] Guid categoryId)
    {
        var result = await _categoryService.GetCategoryAsync(categoryId);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllCategoriesAsync();
        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetCategoriesForUser")] // GetAllForUser old name
    public async Task<IActionResult> GetCategoriesForUser([FromQuery] Guid userId)
    {
        var result = await _categoryService.GetAllCategoriesForUserAsync(userId);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPost("Insert")]
    public async Task<IActionResult> Insert([FromBody] CategoryCreateDto cateogoryRequest)
    {
        var result = await _categoryService.InsertCategoryAsync(cateogoryRequest);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] CategoryUpdateDto categoryUpdateRequest)
    {
        var result = await _categoryService.UpdateCategoryAsync(categoryUpdateRequest);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid categoryId)
    {
        await _categoryService.DeleteCategoryAsync(categoryId);
        return Ok();
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPost("ImageUpdate")]
    public async Task<IActionResult> ImageUpdate(IFormFile file, Guid categoryId)
    {
        var result = await _categoryService.UpdateImageAsync(file, categoryId);
        return Ok(result);
    }

    [Authorize(Roles = "Admin, Authorized")]
    [HttpDelete("ImageDelete")]
    public async Task<IActionResult> ImageDelete(Guid categoryId, string url)
    {
        var result = await _categoryService.DeleteImageAsync(categoryId, url);
        return Ok(result);
    }
}