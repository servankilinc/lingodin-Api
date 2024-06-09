using Business.Abstract;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.UserDtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService) => _userService = userService;

     
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }
     
    [HttpPost("GetUserListByDetail")]
    public async Task<IActionResult> GetUserListByDetail(FSPModel fSPModel)
    {
        var result = await _userService.GetAllUserDetailListAsync(fSPModel);
        return Ok(result);
    }

    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
    {
        var result = await _userService.UpdateUserAsync(userUpdateDto);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid userId)
    { 
        await _userService.DeleteUserAsync(userId);
        return Ok();
    }
}