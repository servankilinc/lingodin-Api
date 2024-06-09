using Core.Model;

namespace Model.Dtos.RoleDtos;

public class RoleResponseDto : IDto
{
    public Guid Id { get; set; } 
    public string? Name { get; set; }
}