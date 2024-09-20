using System.Text.Json.Nodes;

namespace ApiHawk.Core;

public class ResponseType
{
    public int StatusCode { get; set; }
    public string? Headers { get; set; }
    public JsonNode? JsonResponse { get; set; }
    public string? StringResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsError { get; set; }

    public ResponseType(int statusCode, JsonNode? jsonResponse, string? headers, string? stringResponse = null)
    {
        StatusCode = statusCode;
        JsonResponse = jsonResponse;
        Headers = headers;
        StringResponse = stringResponse;
        IsError = false;
    }

    public ResponseType(string? errorMessage)
    {
        IsError = true;
        ErrorMessage = errorMessage;
    }

    public static bool operator true(ResponseType r)
    {
        return !r.IsError;
    }

    public static bool operator false(ResponseType r)
    {
        return r.IsError;
    }
}
