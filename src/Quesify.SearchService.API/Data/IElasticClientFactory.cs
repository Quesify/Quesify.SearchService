using Nest;

namespace Quesify.SearchService.API.Data;

public interface IElasticClientFactory
{
    ElasticClient Create();
}
