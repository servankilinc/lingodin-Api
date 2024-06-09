using Core.Model;
using FluentValidation;

namespace Model.Dtos.RoleDtos;

public class RoleUserRequestDto : IDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}


public class RoleUserRequestDtoValidator : AbstractValidator<RoleUserRequestDto>
{
    public RoleUserRequestDtoValidator()
    {
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.RoleId).NotNull();
    }
}