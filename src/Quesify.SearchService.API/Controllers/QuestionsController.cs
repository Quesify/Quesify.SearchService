using Microsoft.AspNetCore.Mvc;
using Nest;
using Quesify.SearchService.API.Aggregates.Questions;
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

        var response = new List<SearchForQuestionResponse>();
        foreach (var question in questions.Documents)
        {
            response.Add(new SearchForQuestionResponse()
            {
                Body = question.Body,
                Title = question.Title,
                QuestionScore = question.Score,
            });
        }

        var result = new PaginateList<SearchForQuestionResponse>(response, request.Page, request.Size, (int)questions.Total);
        return OkResponse(data: result);
    }
}
