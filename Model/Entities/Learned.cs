using Core.Model;

namespace Model.Entities;

public class Learned : IEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid WordId { get; set; }
    public Word? Word { get; set; }
}