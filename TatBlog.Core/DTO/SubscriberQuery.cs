namespace TatBlog.Core.DTO;

public class SubscriberQuery
{
    public string Keyword { get; set; }

    public string Email { get; set; }

    public bool ForceLock { get; set; }

    public bool UnsubscribeVoluntary { get; set; }
}