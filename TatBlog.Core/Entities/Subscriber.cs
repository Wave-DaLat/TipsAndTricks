using TatBlog.Core.Constracts;

namespace TatBlog.Core.Entities;

public class Subscriber : IEntity
{
    public int Id { get; set; }

    public string SubscribeEmail { get; set; }

    public DateTime SubDated { get; set; }

    public DateTime UnSubDated { get; set; }

    public string CancelReason { get; set; }

    public bool ForceLock { get; set; }

    public string AdminNotes { get; set; }
}