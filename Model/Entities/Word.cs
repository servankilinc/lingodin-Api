using Core.Model;

namespace Model.Entities;

public class Word : IEntity
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string? Turkish { get; set; }
    public string? English { get; set; }
    public bool HasImage { get; set; } = false;
    public string? Image { get; set; }

    public Category? Category { get; set; }
    public ICollection<Favorite>? Favorites { get; set; }
    public ICollection<Learned>? Learneds { get; set; }
}