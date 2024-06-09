using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.WordDtos;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LearningController : ControllerBase
{
    private readonly IWordService _wordService;
    public LearningController(IWordService wordService) => _wordService = wordService;


    [HttpGet("GetLearnedWordsForUser")]
    public async Task<IActionResult> GetLearnedWordsForUser([FromQuery] Guid userId)
    {
        var result = await _wordService.GetLearnedWordsForUserAsync(userId);
        return Ok(result);
    }

    [HttpPost("AddWordAsLearned")] // old name InsertLearnedWords
    public async Task<IActionResult> AddWordAsLearned([FromBody] LearningWordRequestDto learningWordRequest)
    {
        await _wordService.AddWordAsLearnedAsync(learningWordRequest);
        return Ok();
    }

    [HttpPost("RemoveWordFromLearned")] // old name DeleteLearnedWords
    public async Task<IActionResult> RemoveWordFromLearned([FromBody] LearningWordRequestDto learningWordRequest)
    {
        await _wordService.RemoveWordFromLearnedAsync(learningWordRequest);
        return Ok();
    }
}