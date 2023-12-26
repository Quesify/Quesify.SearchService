using Nest;
using Quesify.SearchService.API.Constant;
using Quesify.SharedKernel.Core.Entities;

namespace Quesify.SearchService.API.Aggregates.Questions;

[ElasticsearchType(IdProperty = "id", RelationName = QuestionConstants.IndexName)]
public class Question : AggregateRoot
{
    [PropertyName("id")]
    public Guid Id { get; set; }

    [PropertyName("title")]
    public string Title { get; set; }

    [PropertyName("body")]
    public string Body { get; set; }

    [PropertyName("user_id")]
    public Guid UserId { get; set; }

    [PropertyName("score")]
    public int Score { get; set; }

    [PropertyName("creation_date")]
    public DateTime CreationDate { get; set; }

    public Question()
    {
        Title = null!;
        Body = null!;
    }
}
