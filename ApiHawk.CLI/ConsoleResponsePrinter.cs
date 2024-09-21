using ApiHawk.Core;

namespace ApiHawk.CLI;

public class ConsoleResponsePrinter : IResponsePrinter
{
    public void PrintStandard(ResponseType response)
    {
        switch (response.StatusCode)
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

        Console.WriteLine($"Status Code: {response.StatusCode}");
        Console.ResetColor();
        Console.WriteLine(
            response.JsonResponse != null
                ? $"Response Body:\n{response.JsonResponse}"
                : $"Response Body:\n{response.StringResponse}");
    }

    public void PrintHeaders(ResponseType response)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        if (response.Headers != null)
        {
            Console.WriteLine("Headers:");
            foreach (var c in response.Headers)
            {
                Console.Write(c);
                switch (c)
                {
                    case ':':
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case '\n':
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                }
            }
        }
    }

    public void PrintException(ResponseType response)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("An exception occurred during a HTTP Request:");
        Console.ResetColor();
        Console.WriteLine(response.ErrorMessage);
    }
}
