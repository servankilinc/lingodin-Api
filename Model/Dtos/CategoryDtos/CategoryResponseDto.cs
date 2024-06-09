using Core.Model;

namespace Model.Dtos.CategoryDtos;

public class CategoryResponseDto : IDto
{
    public Guid Id { get; set; }
    public string? Turkish { get; set; }
    public string? English { get; set; }
    public bool HasImage { get; set; }
    public string? Image { get; set; }
}