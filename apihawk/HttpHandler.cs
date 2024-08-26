using System.Text;
using System.Text.Json.Nodes;

namespace API_Tester;

public enum HttpRequestType
{
    Get,
    Post,
    Delete,
    Put
}

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

public class HttpHandler
{
    private readonly HttpClient _httpClient;

    public HttpHandler()
    {
        _httpClient = new HttpClient();
    }

    public async Task<ResponseType> Request(HttpRequest request)
    {
        var httpClient = new HttpClient();
        HttpResponseMessage? response = null;

        try
        {
            switch (request.Type)
            {
                case HttpRequestType.Get:
                    response = await httpClient.GetAsync(request.Url);
                    break;
                case HttpRequestType.Post:
                    if (request.Body != null)
                    {
                        var postContent = new StringContent(request.Body, Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync(request.Url, postContent);
                    }
                    break;
                case HttpRequestType.Delete:
                    response = await httpClient.DeleteAsync(request.Url);
                    break;
                case HttpRequestType.Put:
                    if (request.Body != null)
                    {
                        var putContent = new StringContent(request.Body, Encoding.UTF8, "application/json");
                        response = await httpClient.PutAsync(request.Url, putContent);
                    }
                    break;
            }

            if (response != null)
            {
                var statusCode = (int)response.StatusCode;
                var headers = response.Headers.ToString();
                var responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonObject.Parse(responseBody);
                    return new ResponseType(
                        statusCode: statusCode,
                        jsonResponse: json,
                        stringResponse: null,
                        headers: headers);
                }
                catch
                {
                    return new ResponseType(
                        statusCode: statusCode,
                        jsonResponse: null,
                        stringResponse: responseBody,
                        headers: headers);
                }
            }

            return new ResponseType("Response was null.");
        }
        catch (Exception e)
        {
            return new ResponseType(e.Message);
        }
    }
}
