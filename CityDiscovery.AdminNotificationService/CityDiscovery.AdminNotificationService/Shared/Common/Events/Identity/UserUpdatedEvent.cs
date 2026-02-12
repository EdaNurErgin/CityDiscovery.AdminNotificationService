// Shared.Common/Events/Identity/UserUpdatedEvent.cs
namespace IdentityService.Shared.MessageBus.Identity;
public class UserUpdatedEvent
{
    public Guid UserId { get; set; }
    public string NewUserName { get; set; }
    public string NewAvatarUrl { get; set; }
}