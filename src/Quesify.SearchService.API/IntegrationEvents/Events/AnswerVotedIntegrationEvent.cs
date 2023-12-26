using Quesify.EventBus.Events;

namespace Quesify.SearchService.API.IntegrationEvents.Events;

public class AnswerVotedIntegrationEvent : IntegrationEvent
{
    public Guid AnswerOwnerUserId { get; set; }

    public int UserScoreAction { get; set; }

    public AnswerVotedIntegrationEvent(
       Guid answerOwnerUserId,
       int userScoreAction,
       Guid id,
       DateTime creationDate)
       : base(id, creationDate)
    {
        AnswerOwnerUserId = answerOwnerUserId;
        UserScoreAction = userScoreAction;
    }
}
