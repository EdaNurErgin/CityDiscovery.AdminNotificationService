namespace IdentityService.Shared.MessageBus.Identity;

    public record UserCreatedEvent(
        Guid UserId,
        string UserName,
        string Email,
        string Role,
        DateTime CreatedAtUtc
    );


