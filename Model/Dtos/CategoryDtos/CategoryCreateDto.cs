using Core.Model;
using FluentValidation;

namespace Model.Dtos.CategoryDtos;

public class CategoryCreateDto : IDto
{
    public string Turkish { get; set; } = null!;
    public string English { get; set; } = null!;
}


public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Turkish).NotEmpty();
        RuleFor(x => x.English).NotEmpty();
    }
}