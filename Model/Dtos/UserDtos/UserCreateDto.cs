using Core.Model;
using Core.Utils.Auth;
using FluentValidation;

namespace Model.Dtos.UserDtos;

public class UserCreateDto : IDto
{
    public string? FullName { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public AutheticatorType AutheticatorType { get; set; } = AutheticatorType.None;
}


public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}