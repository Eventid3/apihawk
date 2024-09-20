namespace ApiHawk.Core;

public class HttpRequest
{
    public HttpRequest(HttpRequestType type, string url, string? body = null)
    {
        Type = type;
        Url = url;
        Body = body;
    }

    public HttpRequestType Type { get; set; }
    public string Url { get; set; }
    public string? Body { get; set; }
}