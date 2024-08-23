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

public class HttpHandler
{
    private HttpClient _httpClient;

    public HttpHandler()
    {
        _httpClient = new HttpClient();
    }

    public async Task<ResponseType> Call(HttpRequestType request, string url, string? body = null)
    {
        var httpClient = new HttpClient();
        HttpResponseMessage? response = null;

        try
        {
            switch (request)
            {
                case HttpRequestType.Get:
                    response = await httpClient.GetAsync(url);
                    break;
                case HttpRequestType.Post:
                    if (body != null)
                    {
                        var postContent = new StringContent(body, Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync(url, postContent);
                    }

                    break;
                case HttpRequestType.Delete:
                    response = await httpClient.DeleteAsync(url);
                    break;
                case HttpRequestType.Put:
                    if (body != null)
                    {
                        var putContent = new StringContent(body, Encoding.UTF8, "application/json");
                        response = await httpClient.PutAsync(url, putContent);
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
