using Quesify.EventBus.Events;

namespace Quesify.SearchService.API.IntegrationEvents.Events;

public class QuestionCreatedIntegrationEvent : IntegrationEvent
{
    public Guid QuestionId { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public Guid UserId { get; set; }

    public DateTime QuestionCreationDate { get; set; }

    public QuestionCreatedIntegrationEvent(
        Guid questionId,
        string title,
        string body,
        Guid userId,
        DateTime questionCreationDate,
        Guid id,
        DateTime creationDate)
        : base(id, creationDate)
    {
        QuestionId = questionId;
        Title = title;
        Body = body;
        UserId = userId;
        QuestionCreationDate = questionCreationDate;
    }
}
