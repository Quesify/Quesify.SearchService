using Nest;

namespace Quesify.SearchService.API.Data;

public class ElasticClientFactory : IElasticClientFactory
{
    private readonly ConnectionSettings _connectionSettings;

    public ElasticClientFactory()
    {
        _connectionSettings = new ConnectionSettings();
    }

    public ElasticClient Create()
    {
        return new ElasticClient(_connectionSettings);
    }
}
