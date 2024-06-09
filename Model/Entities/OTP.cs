using Core.Model;

namespace Model.Entities;

public class OTP : IEntity
{
    public Guid UserId { get; set; }
    public DateTime ExpiryTime { get; set; }
    public string? Code { get; set; }
}