using Nest;
using Quesify.SearchService.API.Constant;
using Quesify.SharedKernel.Core.Entities;

namespace Quesify.SearchService.API.Aggregates.Users;

[ElasticsearchType(IdProperty = "id", RelationName = QuestionConstants.IndexName)]
public class User : AggregateRoot
{
    [PropertyName("id")]
    public Guid Id { get; set; }

    [PropertyName("user_name")]
    public string UserName { get; set; }

    [PropertyName("score")]
    public int Score { get; set; }

    [PropertyName("profile_image_url")]
    public string? ProfileImageUrl { get; set; }

    public User()
    {
        UserName = null!;
    }
}
