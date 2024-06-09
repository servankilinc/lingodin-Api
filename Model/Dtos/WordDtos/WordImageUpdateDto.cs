using Core.Model;
using FluentValidation;

namespace Model.Dtos.WordDtos;

public class WordImageUpdateDto : IDto
{
    public Guid Id { get; set; }
    public bool HasImage { get; set; } = false;
    public string? Image { get; set; }
}


public class WordImageUpdateDtoValidator : AbstractValidator<WordImageUpdateDto>
{
    public WordImageUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.HasImage).NotNull();
        RuleFor(x => x.Image).NotEmpty().When(x => x.HasImage == true);
    }
}