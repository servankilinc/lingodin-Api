using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.RoleDtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService) => _roleService = roleService;


    [Authorize]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _roleService.GetAllRolesAsync();
        return Ok(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("GetAllByUser")]
    public async Task<IActionResult> GetAllByUser([FromQuery] Guid userId)
    {
        var result = await _roleService.GetAllRolesByUserAsync(userId);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("GetUserRoles")]
    public async Task<IActionResult> GetUserRoles([FromQuery] Guid userId) //  old name : GetRolesForUser 
    {
        var result = await _roleService.GetUserRolesAsync(userId);
        return Ok(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("InsertRole")]
    public async Task<IActionResult> InsertRole([FromBody] RoleCreateDto roleRequest)
    {
        var result = await _roleService.InsertRoleAsync(roleRequest);
        return Ok(result);
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid roleId)
    {
        await _roleService.DeleteRoleAsync(roleId);
        return Ok();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("AddRoleToUser")]
    public async Task<IActionResult> AddRoleToUser([FromBody] RoleUserRequestDto roleUserRequest)
    {
        await _roleService.AddRoleToUserAsync(roleUserRequest);
        return Ok();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("RemoveRoleFromUser")] //old name DeleteRoleToUser
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RoleUserRequestDto roleUserRequest)
    {
        await _roleService.RemoveRoleFromUserAsync(roleUserRequest);
        return Ok();
    }
}