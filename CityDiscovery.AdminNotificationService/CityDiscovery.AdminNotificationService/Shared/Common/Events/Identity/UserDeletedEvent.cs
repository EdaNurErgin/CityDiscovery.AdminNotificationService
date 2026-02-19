namespace IdentityService.Shared.MessageBus.Identity
{
    public record UserDeletedEvent(
        Guid UserId,
        string UserName,
        string Email,
        string Role,
        DateTime DeletedAtUtc
    );
}