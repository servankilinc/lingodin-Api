using Core.Model;
using Core.Utils.Auth;
using FluentValidation;

namespace Model.Dtos.UserDtos;

public class UserUpdateDto : IDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
}


public class UserDetailUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserDetailUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}