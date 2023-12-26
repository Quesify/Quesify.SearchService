using Quesify.EventBus.Events;

namespace Quesify.SearchService.API.IntegrationEvents.Events;

public class UserUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; set; }

    public string? ProfileImageUrl { get; set; }

    public UserUpdatedIntegrationEvent(
       Guid userId,
       string? profileImageUrl,
       Guid id,
       DateTime creationDate)
       : base(id, creationDate)
    {
        UserId = userId;
        ProfileImageUrl = profileImageUrl;
    }
}
