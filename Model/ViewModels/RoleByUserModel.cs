namespace Model.ViewModels;

public class RoleByUserModel
{
    public Guid RoleId { get; set; }
    public string? RoleName { get; set; }
    public Guid UserId { get; set; }
    public bool IsUserHave { get; set; }

    public RoleByUserModel(Guid roleId, string? roleName, Guid userId, bool ısUserHave)
    {
        RoleId = roleId;
        RoleName = roleName;
        UserId = userId;
        IsUserHave = ısUserHave;
    }
}