using Quesify.EventBus.Events;

namespace Quesify.SearchService.API.IntegrationEvents.Events;

public class QuestionVotedIntegrationEvent : IntegrationEvent
{
    public Guid QuestionId { get; set; }

    public Guid QuestionOwnerUserId { get; set; }

    public int NewQuestionScore { get; set; }

    public int UserScoreAction { get; set; }

    public QuestionVotedIntegrationEvent(
        Guid questionId,
        Guid questionOwnerUserId,
        int newQuestionScore,
        int userScoreAction,
        Guid id,
        DateTime creationDate)
        : base(id, creationDate)
    {
        QuestionId = questionId;
        QuestionOwnerUserId = questionOwnerUserId;
        NewQuestionScore = newQuestionScore;
        UserScoreAction = userScoreAction;
    }
}
