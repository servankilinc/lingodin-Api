using Core.Model;

namespace Model.Entities;

public class Role : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<UserRoles>? UserRoles { get; set; }
}