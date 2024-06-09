using Core.Model;

namespace Model.Dtos.WordDtos;

public class WordResponseDto: IDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string? Turkish { get; set; }
    public string? English { get; set; }
    public bool HasImage { get; set; } = false;
    public string? Image { get; set; }
}