using Quesify.SharedKernel.Core.Entities;

namespace Quesify.SearchService.API.Aggregates.Users;

public class User : AggregateRoot
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string? ProfileImageUrl { get; set; }

    public User()
    {
        UserName = null!;
    }
}
