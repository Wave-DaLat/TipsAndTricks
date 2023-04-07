namespace TatBlog.WebApi.Models;

public class ValidationFailureResponse
{
    public IEnumerable<string> Erros { get; set; }

    public ValidationFailureResponse(
        IEnumerable<string> errosMessages)
    {
        Erros = errosMessages;
    }
}
