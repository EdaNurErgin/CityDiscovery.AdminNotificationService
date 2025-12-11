// Shared.Common/Events/Identity/UserUpdatedEvent.cs
public class UserUpdatedEvent
{
    public Guid UserId { get; set; }
    public string NewUserName { get; set; }
    public string NewAvatarUrl { get; set; }
}