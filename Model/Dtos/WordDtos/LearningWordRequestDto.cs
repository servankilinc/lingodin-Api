using Core.Model;
using FluentValidation;

namespace Model.Dtos.WordDtos;

public class LearningWordRequestDto : IDto
{
    public Guid UserId { get; set; }
    public Guid WordId { get; set; }
}


public class LearningWordRequestDtoValidator : AbstractValidator<LearningWordRequestDto>
{
    public LearningWordRequestDtoValidator()
    {
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.WordId).NotNull();
    }
}