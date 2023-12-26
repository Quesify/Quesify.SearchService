using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
using Quesify.SearchService.API.Aggregates.Users;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.SearchService.API.IntegrationEvents.EventHandlers;

public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    private readonly ElasticClient _elasticClient;
    private readonly ILogger<QuestionCreatedIntegrationEventHandler> _logger;

    public UserCreatedIntegrationEventHandler(
        IElasticClientFactory elasticClientFactory,
        ILogger<QuestionCreatedIntegrationEventHandler> logger)
    {
        _elasticClient = elasticClientFactory.Create();
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedIntegrationEvent integrationEvent)
    {
        var user = new User()
        {
            Id = integrationEvent.UserId,
            UserName = integrationEvent.UserName,
            Score = 0,
            ProfileImageUrl = null
        };

        var response = await _elasticClient.IndexAsync(user, o => o.Index(UserConstants.IndexName));
        if (!response.IsValid)
        {
            _logger.LogError("Elasticsearch user index error: {message}", response.ServerError.Error.ToString());
        }
    }
}
