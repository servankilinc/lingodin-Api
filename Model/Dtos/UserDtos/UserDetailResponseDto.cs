using Core.Model;
using Model.Dtos.RoleDtos;

namespace Model.Dtos.UserDtos;

public class UserDetailResponseDto : IDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool IsVerifiedUser { get; set; }
    public IEnumerable<RoleResponseDto>? Roles { get; set; }


    public UserDetailResponseDto(Guid id, string fullName, string email, bool isVerifiedUser)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        IsVerifiedUser = isVerifiedUser;

    }
    public UserDetailResponseDto(Guid id, string fullName, string email, bool isVerifiedUser, IEnumerable<RoleResponseDto>? roles)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        IsVerifiedUser = isVerifiedUser;
        Roles = roles;
    }
}