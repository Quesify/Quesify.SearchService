using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.SearchService.API.IntegrationEvents.EventHandlers;

public class QuestionCreatedIntegrationEventHandler : IIntegrationEventHandler<QuestionCreatedIntegrationEvent>
{
    private readonly ElasticClient _elasticClient;
    private readonly ILogger<QuestionCreatedIntegrationEventHandler> _logger;

    public QuestionCreatedIntegrationEventHandler(
        IElasticClientFactory elasticClientFactory,
        ILogger<QuestionCreatedIntegrationEventHandler> logger)
    {
        _elasticClient = elasticClientFactory.Create();
        _logger = logger;
    }

    public async Task HandleAsync(QuestionCreatedIntegrationEvent integrationEvent)
    {
        var question = new Question()
        {
            Id = integrationEvent.QuestionId,
            Title = integrationEvent.Title,
            Body = integrationEvent.Body,
            UserId = integrationEvent.UserId,
            Score = 0,
            CreationDate = integrationEvent.CreationDate
        };

        var response = await _elasticClient.IndexAsync(question, o => o.Index(QuestionConstants.IndexName));
        if (!response.IsValid)
        {
            _logger.LogError("Elasticsearch question index error: {message}", response.ServerError.Error.ToString());
        }
    }
}
