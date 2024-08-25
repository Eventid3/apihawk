namespace API_Tester;

public class BatchItem
{
    public BatchItem(string type, string options, string url, string? body = null)
    {
        switch (type)
        {
            case "get":
                Type = HttpRequestType.Get;
                break;
            case "post":
                Type = HttpRequestType.Post;
                break;
            case "put":
                Type = HttpRequestType.Put;
                break;
            case "delete":
                Type = HttpRequestType.Delete;
                break;
        }

        Options = options;
        Url = url;
        Body = body;
    }

    public HttpRequestType Type { get; set; }
    public string Options { get; set; }
    public string Url { get; set; }
    public string? Body { get; set; }
}
