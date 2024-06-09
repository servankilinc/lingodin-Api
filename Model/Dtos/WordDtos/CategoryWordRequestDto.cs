using Core.Model;
using FluentValidation;

namespace Model.Dtos.WordDtos;

public class CategoryWordRequestDto : IDto
{
    public Guid WordId { get; set; }
    public Guid CategoryId { get; set; }
}


public class CategoryWordRequestDtoValidator : AbstractValidator<CategoryWordRequestDto>
{
    public CategoryWordRequestDtoValidator()
    {
        RuleFor(x => x.WordId).NotNull();
        RuleFor(x => x.CategoryId).NotNull();
    }
}