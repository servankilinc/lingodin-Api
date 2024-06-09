using Core.Model;
using FluentValidation;

namespace Model.Dtos.CategoryDtos;

public class CategoryUpdateDto : IDto
{
    public Guid Id { get; set; }
    public string Turkish { get; set; } = null!;
    public string English { get; set; } = null!;
}


public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Turkish).NotEmpty();
        RuleFor(x => x.English).NotEmpty();
    }
}