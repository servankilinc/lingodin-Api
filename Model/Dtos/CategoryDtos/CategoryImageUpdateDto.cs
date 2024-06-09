using Core.Model;
using FluentValidation;

namespace Model.Dtos.CategoryDtos;

public class CategoryImageUpdateDto : IDto
{
    public Guid Id { get; set; } 
    public bool HasImage { get; set; } = false;
    public string? Image { get; set; }
}


public class CategoryImageUpdateDtoValidator : AbstractValidator<CategoryImageUpdateDto>
{
    public CategoryImageUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.HasImage).NotNull();
        RuleFor(x => x.Image).NotEmpty().When(x => x.HasImage == true);
    }
}