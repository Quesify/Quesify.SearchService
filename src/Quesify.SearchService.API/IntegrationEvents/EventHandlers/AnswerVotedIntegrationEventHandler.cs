using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
using Quesify.SearchService.API.Aggregates.Users;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.SearchService.API.IntegrationEvents.EventHandlers;

public class AnswerVotedIntegrationEventHandler : IIntegrationEventHandler<AnswerVotedIntegrationEvent>
{
    private readonly ElasticClient _elasticClient;
    private readonly ILogger<QuestionCreatedIntegrationEventHandler> _logger;

    public AnswerVotedIntegrationEventHandler(
        IElasticClientFactory elasticClientFactory,
        ILogger<QuestionCreatedIntegrationEventHandler> logger)
    {
        _elasticClient = elasticClientFactory.Create();
        _logger = logger;
    }

    public async Task HandleAsync(AnswerVotedIntegrationEvent integrationEvent)
    {
        var user = (await _elasticClient.GetAsync<User>(integrationEvent.AnswerOwnerUserId, o => o.Index(UserConstants.IndexName))).Source;
        if (user == null)
        {
            _logger.LogError("The user {UserId} was not found.", integrationEvent.AnswerOwnerUserId);
            return;
        }

        user.Score += integrationEvent.UserScoreAction;

        var userUpdateResponse = await _elasticClient
            .UpdateAsync<User>(
                integrationEvent.AnswerOwnerUserId,
                selector: o => o.Index(UserConstants.IndexName).Doc(user)
            );

        if (!userUpdateResponse.IsValid)
        {
            _logger.LogError("Elasticsearch user update error: {message}", userUpdateResponse.ServerError.Error.ToString());
            return;
        }
    }
}
