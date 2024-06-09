using Core.Model;
using Core.Utils.Auth;
using Model.Dtos.RoleDtos;
using Model.Dtos.UserDtos;

namespace Model.ViewModels;

public class UserAuthResponseModel : IDto
{
    public UserResponseDto? User { get; set; }
    public AccessToken? AccessToken { get; set; }
    public ICollection<RoleResponseDto>? Roles { get; set; }
}