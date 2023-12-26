namespace Quesify.SearchService.API.Models;

public class SearchForQuestionRequest
{
    public int Page { get; set; }

    public int Size { get; set; }

    public string? Text { get; set; }

    public Guid? UserId { get; set; }

    public int? Score { get; set; }

    public SearchForQuestionRequest()
    {
        Page = 1;
        Size = 10;
    }
}
