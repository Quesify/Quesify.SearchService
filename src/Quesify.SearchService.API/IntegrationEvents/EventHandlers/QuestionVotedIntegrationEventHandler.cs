using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;
using Quesify.SharedKernel.Utilities.Exceptions;

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
}
