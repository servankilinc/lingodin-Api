using FluentValidation;

namespace Model.Dtos.UserDtos;

public class UserPasswordResetDto
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = null!;
    public string ConfirmedPassword { get; set; } = null!;
    public string VerifyAgainOtpCode { get; set; } = null!;
}


public class UserPasswordResetDtoValidator : AbstractValidator<UserPasswordResetDto>
{
    public UserPasswordResetDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmedPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.VerifyAgainOtpCode).NotEmpty().MinimumLength(6);

        RuleFor(x => x).Custom((model, context) =>
        {
            if (model.Password != model.ConfirmedPassword)
            {
                context.AddFailure("ConfirmedPassword", "Passwords don't match.");
            }
        });
    }
}