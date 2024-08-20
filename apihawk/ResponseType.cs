using System.Text.Json.Nodes;

namespace API_Tester;

public class ResponseType
{
    public int StatusCode { get; set; }
    public JsonNode? JsonResponse { get; set; }
    public string? StringResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsError { get; set; }

    public ResponseType(int statusCode, JsonNode? jsonResponse, string? stringResponse = null)
    {
        StatusCode = statusCode;
        JsonResponse = jsonResponse;
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

    public void Print()
    {
        switch (StatusCode)
        {
            case >= 200 and <= 299:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case >= 300 and <= 399:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case >= 400 and <= 499:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case >= 500 and <= 599:
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
        }

        Console.WriteLine($"Status Code: {StatusCode}");
        Console.ResetColor();
        Console.WriteLine(
            JsonResponse != null ? $"Response Body:\n{JsonResponse}" : $"Response Body:\n{StringResponse}");
    }

    public void PrintException()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("An exception occurred during a HTTP Request:");
        Console.ResetColor();
        Console.WriteLine(ErrorMessage);
    }
}
