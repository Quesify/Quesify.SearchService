using Quesify.EventBus.Events;

namespace Quesify.SearchService.API.IntegrationEvents.Events;

public class UserCreatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public UserCreatedIntegrationEvent(
        Guid userId,
        string userName,
        Guid id,
        DateTime creationDate)
        : base(id, creationDate)
    {
        UserId = userId;
        UserName = userName;
    }
}
