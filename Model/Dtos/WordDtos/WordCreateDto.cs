using Core.Model;
using FluentValidation;

namespace Model.Dtos.WordDtos;

public class WordCreateDto : IDto
{
    public string Turkish { get; set; } = null!;
    public string English { get; set; } = null!;
    public Guid CategoryId { get; set; }
}


public class WordCreateDtoValidator : AbstractValidator<WordCreateDto>
{
    public WordCreateDtoValidator()
    {
        RuleFor(x => x.Turkish).NotEmpty();
        RuleFor(x => x.English).NotEmpty();
        RuleFor(x => x.CategoryId).NotNull();
    }
}