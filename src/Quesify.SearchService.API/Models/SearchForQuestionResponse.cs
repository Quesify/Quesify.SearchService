namespace Quesify.SearchService.API.Models;

public class SearchForQuestionResponse
{

    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public int QuestionScore { get; set; }

    public SearchForQuestionUserResponse User { get; set; }

    public DateTime CreationDate { get; set; }

    public SearchForQuestionResponse()
    {
        Title = null!;
        Body = null!;
        User = null!;
    }
}

public class SearchForQuestionUserResponse
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string? ProfileImageUrl { get; set; }

    public int Score { get; set; }

    public SearchForQuestionUserResponse()
    {
        UserName = null!;
    }
}