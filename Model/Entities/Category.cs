using Core.Model;
using System.Text.Json.Serialization;

namespace Model.Entities;

public class Category : IEntity
{
    public Guid Id { get; set; }
    public string? Turkish { get; set; }
    public string? English { get; set; }
    public bool HasImage { get; set; } = false;
    public string? Image { get; set; }
    [JsonIgnore]
    public ICollection<Word>? Words { get; set; }
}