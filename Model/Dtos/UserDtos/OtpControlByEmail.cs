using FluentValidation;

namespace Model.Dtos.UserDtos;

public class OtpControlByEmail
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;

    public OtpControlByEmail()
    {
    }
    public OtpControlByEmail(string email, string code)
    {
        Email = email;
        Code = code;
    }
}
public class OtpControlByEmailValidator : AbstractValidator<OtpControlByEmail>
{
    public OtpControlByEmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Code).MinimumLength(6);
    }
}