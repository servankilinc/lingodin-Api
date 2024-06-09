using Core.Utils.Auth;
using Model.Dtos.RoleDtos;

namespace Model.ViewModels;

public class AccessTokenResultModel
{
    public AccessToken? AccessToken { get; set; }
    public ICollection<RoleResponseDto>? Roles { get; set; }
}