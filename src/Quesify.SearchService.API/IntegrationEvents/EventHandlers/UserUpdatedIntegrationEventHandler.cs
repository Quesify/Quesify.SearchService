using Nest;
using Quesify.SearchService.API.Aggregates.Users;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.SearchService.API.IntegrationEvents.EventHandlers;

public class UserUpdatedIntegrationEventHandler : IIntegrationEventHandler<UserUpdatedIntegrationEvent>
{
    private readonly ElasticClient _elasticClient;
    private readonly ILogger<QuestionCreatedIntegrationEventHandler> _logger;

    public UserUpdatedIntegrationEventHandler(
        IElasticClientFactory elasticClientFactory,
        ILogger<QuestionCreatedIntegrationEventHandler> logger)
    {
        _elasticClient = elasticClientFactory.Create();
        _logger = logger;
    }

    public async Task HandleAsync(UserUpdatedIntegrationEvent integrationEvent)
    {
        var user = (await _elasticClient.GetAsync<User>(integrationEvent.UserId, o => o.Index(UserConstants.IndexName))).Source;
        if (user == null)
        {
            _logger.LogError("The user {UserId} was not found.", integrationEvent.UserId);
            return;
        }

        user.ProfileImageUrl = integrationEvent.ProfileImageUrl;

        var userUpdateResponse = await _elasticClient
            .UpdateAsync<User>(
                integrationEvent.UserId,
                selector: o => o.Index(UserConstants.IndexName).Doc(user)
            );

        if (!userUpdateResponse.IsValid)
        {
            _logger.LogError("Elasticsearch user update error: {message}", userUpdateResponse.ServerError.Error.ToString());
            return;
        }
    }
}
