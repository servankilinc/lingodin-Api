using Model.Dtos.RoleDtos;
using Model.ViewModels;

namespace Business.Abstract;

public interface IRoleService
{
    Task<ICollection<RoleResponseDto>> GetAllRolesAsync();
    Task<RoleResponseDto> GetRoleByNameAsync(string name);
    Task<ICollection<RoleResponseDto>> GetUserRolesAsync(Guid userId);
    Task<ICollection<RoleByUserModel>> GetAllRolesByUserAsync(Guid userId);
    Task<RoleResponseDto> InsertRoleAsync(RoleCreateDto roleCreateDto);
    Task DeleteRoleAsync(Guid roleId);
     
    Task RemoveRoleFromUserAsync(RoleUserRequestDto roleUserRequestDto);
    Task AddRoleToUserAsync(RoleUserRequestDto roleUserRequestDto);
}