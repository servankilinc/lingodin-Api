using FluentValidation;

namespace Model.Dtos.UserDtos;

public class OtpControlDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = null!;
    
    public OtpControlDto()
    {
    }
    public OtpControlDto(Guid userId, string code)
    {
        UserId = userId;
        Code = code;
    }
}
public class OtpControlDtoValidator : AbstractValidator<OtpControlDto>
{
    public OtpControlDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).MinimumLength(6);
    }
}