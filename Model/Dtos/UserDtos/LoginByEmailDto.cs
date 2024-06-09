using Core.Model;
using FluentValidation;

namespace Model.Dtos.UserDtos;

public class LoginByEmailDto : IDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}


public class LoginByEmailDtoValidator : AbstractValidator<LoginByEmailDto>
{
    public LoginByEmailDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(6);
    }
}