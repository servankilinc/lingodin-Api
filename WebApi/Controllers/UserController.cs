using Business.Abstract;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.UserDtos;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService) => _userService = userService;


    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("GetUserListByDetail")]
    public async Task<IActionResult> GetUserListByDetail(FSPModel fSPModel)
    {
        var result = await _userService.GetAllUserDetailListAsync(fSPModel);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetUserInfoById")]
    public async Task<IActionResult> GetUserInfoById(Guid userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);
        return Ok(result);
    }


    [Authorize]
    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
    {
        var requesterId = User.Claims.Where(i => i.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        if (requesterId?.Value != userUpdateDto.Id.ToString() || User.IsInRole("Admin"))
        {
            return BadRequest("Authentication");
        }
        var result = await _userService.UpdateUserAsync(userUpdateDto);
        return Ok(result);
    }


    [Authorize]
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid userId)
    {
        var requesterId = User.Claims.Where(i => i.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
        if (requesterId?.Value != userId.ToString() || User.IsInRole("Admin"))
        {
            return BadRequest("Authentication");
        }

        await _userService.DeleteUserAsync(userId);
        return Ok();
    }
}