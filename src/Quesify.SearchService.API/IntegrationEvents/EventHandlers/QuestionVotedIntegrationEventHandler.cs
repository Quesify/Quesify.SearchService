using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
using Quesify.SearchService.API.Aggregates.Users;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.SearchService.API.IntegrationEvents.EventHandlers;

public class QuestionVotedIntegrationEventHandler : IIntegrationEventHandler<QuestionVotedIntegrationEvent>
{
    private readonly ElasticClient _elasticClient;
    private readonly ILogger<QuestionCreatedIntegrationEventHandler> _logger;

    public QuestionVotedIntegrationEventHandler(
        IElasticClientFactory elasticClientFactory,
        ILogger<QuestionCreatedIntegrationEventHandler> logger)
    {
        _elasticClient = elasticClientFactory.Create();
        _logger = logger;
    }

    public async Task HandleAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        await UpdateNewQuestionScoreAsync(integrationEvent);
        await UpdateUserScoreAsync(integrationEvent);
    }

    private async Task UpdateNewQuestionScoreAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        var question = (await _elasticClient.GetAsync<Question>(integrationEvent.QuestionId, o => o.Index(QuestionConstants.IndexName))).Source;
        if (question == null)
        {
            _logger.LogError("The question {QuestionId} was not found.", integrationEvent.QuestionId);
            return;
        }

        question.Score = integrationEvent.NewQuestionScore;

        var questionUpdateResponse = await _elasticClient
            .UpdateAsync<Question>(
                integrationEvent.QuestionId,
                selector: o => o.Index(QuestionConstants.IndexName).Doc(question)
            );

        if (!questionUpdateResponse.IsValid)
        {
            _logger.LogError("Elasticsearch question update error: {message}", questionUpdateResponse.ServerError.Error.ToString());
            return;
        }
    }

    private async Task UpdateUserScoreAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        var user = (await _elasticClient.GetAsync<User>(integrationEvent.QuestionOwnerUserId, o => o.Index(UserConstants.IndexName))).Source;
        if (user == null)
        {
            _logger.LogError("The user {UserId} was not found.", integrationEvent.QuestionOwnerUserId);
            return;
        }

        user.Score += integrationEvent.UserScoreAction;

        var userUpdateResponse = await _elasticClient
            .UpdateAsync<User>(
                integrationEvent.QuestionOwnerUserId,
                selector: o => o.Index(UserConstants.IndexName).Doc(user)
            );

        if (!userUpdateResponse.IsValid)
        {
            _logger.LogError("Elasticsearch user update error: {message}", userUpdateResponse.ServerError.Error.ToString());
            return;
        }
    }
}
