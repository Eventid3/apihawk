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

        try
        {
            HttpResponseMessage? response = null;
            switch (request)
            {
                case HttpRequestType.Get:
                    Console.WriteLine("HttpRequest: GET");
                    response = await httpClient.GetAsync(url);
                    break;
                case HttpRequestType.Post:
                    Console.WriteLine("HttpRequest: POST");
                    if (body != null)
                    {
                        var postContent = new StringContent(body, Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync(url, postContent);
                    }

                    break;
                case HttpRequestType.Delete:
                    Console.WriteLine("HttpRequest: DELETE");
                    response = await httpClient.DeleteAsync(url);
                    break;
                case HttpRequestType.Put:
                    Console.WriteLine("HttpRequest: PUT");
                    if (body != null)
                    {
                        var putContent = new StringContent(body, Encoding.UTF8, "application/json");
                        response = await httpClient.PutAsync(url, putContent);
                    }

                    break;
            }

            if (response != null)
            {
                Console.WriteLine("Response wasn't null");
                var statusCode = (int)response.StatusCode;
                var responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonObject.Parse(responseBody);
                    return new ResponseType(statusCode, json);
                }
                catch
                {
                    return new ResponseType(statusCode, null, responseBody);
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
