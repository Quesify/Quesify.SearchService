using Microsoft.AspNetCore.Mvc;
using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
using Quesify.SearchService.API.Aggregates.Users;
using Quesify.SearchService.API.Constant;
using Quesify.SearchService.API.Data;
using Quesify.SearchService.API.Models;
using Quesify.SharedKernel.AspNetCore.Controllers;
using Quesify.SharedKernel.Utilities.Pagination;

namespace Quesify.SearchService.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class QuestionsController : BaseController
{
    private readonly IElasticClient _elasticClient;

    public QuestionsController(IElasticClientFactory elasticClientFactory)
    {
        _elasticClient = elasticClientFactory.Create();
    }

    [HttpGet]
    public async Task<IActionResult> SearchQuestion([FromQuery] SearchForQuestionRequest request)
    {
        var questions = await _elasticClient.SearchAsync<Question>(selector =>
            selector
                .Index(QuestionConstants.IndexName)
                .From((request.Page - 1) * request.Size)
                .Size(request.Size)
                .Query(query =>
                    query
                        .MultiMatch(match => match
                            .Query(request.Text)
                            .Operator(Operator.And)
                            .Fields(field => field.Field(o => o.Title).Field(o => o.Body))) &&
                    query
                        .Term(term => term
                            .Value(request.UserId)
                            .Field(field => field.UserId)) &&
                    query
                        .Range(range => range
                            .GreaterThanOrEquals(request.Score)
                            .Field(field => field.Score))
                )
            );

        var userIds = questions.Documents
           .Select(o => o.UserId)
           .Distinct()
           .ToArray();

        var users = (await _elasticClient
            .SearchAsync<User>(selector => selector
                .Index(UserConstants.IndexName)
                .Query(query => query
                    .Ids(i => i
                        .Values(userIds))
                )
            )).Documents;

        var response = new List<SearchForQuestionResponse>();
        foreach (var question in questions.Documents)
        {
            var user = users.FirstOrDefault(o => o.Id == question.UserId)!;
            response.Add(new SearchForQuestionResponse()
            {
                Id = question.Id,
                Body = question.Body,
                Title = question.Title,
                QuestionScore = question.Score,
                User = new SearchForQuestionUserResponse()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    ProfileImageUrl = user.ProfileImageUrl,
                    Score = user.Score,
                },
                CreationDate = question.CreationDate
            });
        }

        var countRequest = new CountRequest(Indices.Index(QuestionConstants.IndexName));
        long totalCount = (await _elasticClient.CountAsync(countRequest)).Count;

        var result = new PaginateList<SearchForQuestionResponse>(response, request.Page, request.Size, (int)totalCount);
        return OkResponse(data: result);
    }
}
