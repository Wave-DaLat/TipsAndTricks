namespace TatBlog.WebApi.Models;

public class SubscriberEditModel
{
    public string SubscribeEmail { get; set; }

    public string CancelReason { get; set; }

    public bool ForceLock { get; set; }

    public bool UnsubscribeVoluntary { get; set; }

    public string AdminNotes { get; set; }
}
