using FluentValidation;
using Microsoft.Extensions.Azure;

namespace Model.Dtos.UserDtos;

public class UserPasswordResetDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmedPassword { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
}


public class UserPasswordResetDtoValidator : AbstractValidator<UserPasswordResetDto>
{
    public UserPasswordResetDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmedPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.OtpCode).NotEmpty().MinimumLength(6);

        RuleFor(x => x).Custom((model, context) =>
        {
            if (model.Password != model.ConfirmedPassword)
            {
                context.AddFailure("ConfirmedPassword", "Passwords don't match.");
            }
        });
    }
}