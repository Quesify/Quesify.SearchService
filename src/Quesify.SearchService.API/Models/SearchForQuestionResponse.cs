namespace Quesify.SearchService.API.Models;

public class SearchForQuestionResponse
{
    public string Title { get; set; }

    public string Body { get; set; }

    public int QuestionScore { get; set; }

    public SearchForQuestionResponse()
    {
        Title = null!;
        Body = null!;
    }
}
