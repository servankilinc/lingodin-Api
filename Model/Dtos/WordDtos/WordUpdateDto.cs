using Core.Model;
using FluentValidation;

namespace Model.Dtos.WordDtos;

public class WordUpdateDto : IDto
{
    public Guid Id { get; set; }
    public string Turkish { get; set; } = null!;
    public string English { get; set; } = null!;
    public Guid CategoryId { get; set; }
}


public class WordUpdateDtoValidator : AbstractValidator<WordUpdateDto>
{
    public WordUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Turkish).NotEmpty();
        RuleFor(x => x.English).NotEmpty();
        RuleFor(x => x.CategoryId).NotNull();
    }
}