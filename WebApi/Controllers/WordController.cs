using Business.Abstract;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.WordDtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordController : ControllerBase
{
    private readonly IWordService _wordService;
    
    public WordController(IWordService wordService)
    {
        _wordService = wordService;
    }

    [Authorize]
    [HttpGet("Get")]
    public async Task<IActionResult> Get(Guid wordId)
    {
        var result = await _wordService.GetWordAsync(wordId);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPost("GetAll")]
    public async Task<IActionResult> GetAll(FSPModel fSPModel)
    {
        var result = await _wordService.GetAllWordsAsync(fSPModel);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetWordsByCategory")] // GetAllByCategory old name
    public async Task<IActionResult> GetWordsByCategory([FromQuery] Guid categoryId)
    {
        var result = await _wordService.GetWordsByCategoryAsync(categoryId);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetWordsByCategoryForUser")] // GetAllByCategoryForUser old name
    public async Task<IActionResult> GetWordsByCategoryForUser([FromQuery] Guid categoryId, Guid userId)
    {
        var result = await _wordService.GetWordsByCategoryForUserAsync(categoryId, userId);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPost("Insert")]
    public async Task<IActionResult> Insert([FromBody] WordCreateDto wordRequest)
    {
        var result = await _wordService.InsertWordAsync(wordRequest);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] WordUpdateDto wordUpdateRequest)
    {
        var result = await _wordService.UpdateWordAsync(wordUpdateRequest);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid wordId)
    {
        await _wordService.DeleteWordAsync(wordId);
        return Ok();
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpPost("ImageUpdate")]
    public async Task<IActionResult> ImageUpdate(IFormFile file, Guid wordId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "Word");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{wordId}{fileExtension}";
        var filePath = Path.Combine(uploadFolder, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"/Word/{fileName}";
        var result = await _wordService.UpdateImageAsync(wordId, fileUrl);
        return Ok(result);
    }


    [Authorize(Roles = "Admin, Authorized")]
    [HttpDelete("ImageDelete")]
    public async Task<IActionResult> ImageDelete(Guid wordId, string url)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", url.TrimStart('/')); // Başındaki /'yi çıkarıyoruz

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            var result = await _wordService.DeleteImageAsync(wordId);
            return Ok(result);
        }

        return BadRequest("File not found.");
    }
}