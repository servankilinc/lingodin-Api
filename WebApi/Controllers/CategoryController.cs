using Business.Abstract;
using Business.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.CategoryDtos;
using Model.Entities;
using System.Security.Policy;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

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
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "Category");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{categoryId}{fileExtension}";
        var filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"/Category/{fileName}";
        var result = await _categoryService.UpdateImageAsync(categoryId, fileUrl);
        return Ok(result);

    }

    [Authorize(Roles = "Admin, Authorized")]
    [HttpDelete("ImageDelete")]
    public async Task<IActionResult> ImageDelete(Guid categoryId, string url)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", url.TrimStart('/'));

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            var result = await _categoryService.DeleteImageAsync(categoryId);
            return Ok(result);
        }

        return BadRequest("File not found.");
    }
}