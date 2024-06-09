using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.WordDtos;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoriteController : ControllerBase
{
    private readonly IWordService _wordService;
    public FavoriteController(IWordService wordService) => _wordService = wordService;


    [HttpGet("GetFavWordsForUser")]
    public async Task<IActionResult> GetFavWordsForUser([FromQuery] Guid userId)
    {
        var result = await _wordService.GetFavoriteWordsForUserAsync(userId);
        return Ok(result);
    }

    [HttpPost("AddWordAsFavorite")] // old name InsertFavorite
    public async Task<IActionResult> AddWordAsFavorite([FromBody] FavoriteWordRequestDto favoriteWordRequest)
    {
        await _wordService.AddWordAsFavoriteAsync(favoriteWordRequest);
        return Ok();
    }

    [HttpPost("RemoveWordFromFavorites")] // old name DeleteFavorite
    public async Task<IActionResult> RemoveWordFromFavorites([FromBody] FavoriteWordRequestDto favoriteWordRequest)
    {
        await _wordService.RemoveWordFromFavoritesAsync(favoriteWordRequest);
        return Ok();
    }
}