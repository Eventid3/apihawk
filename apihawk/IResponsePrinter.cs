using System.Diagnostics;
using System.Net;

namespace API_Tester;

public interface IResponsePrinter
{
    void PrintStandard(ResponseType response);
    void PrintHeaders(ResponseType response);
    void PrintException(ResponseType response);
}

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

public class FileResponsePrinter : IResponsePrinter
{
    public string? FilePath { get; set; }
    private bool _initialWrite = true;

    public FileResponsePrinter(string? filePath)
    {
        FilePath = filePath;
    }

    public void PrintStandard(ResponseType response)
    {
        Console.WriteLine($"Printing standard info to file {FilePath}");
        Debug.Assert(FilePath != null, nameof(FilePath) + " != null");
        StreamWriter writer;
        if (_initialWrite)
        {
            writer = new StreamWriter(FilePath);
            _initialWrite = false;
            writer.WriteLine($"Response, date {DateTime.Now}:");
        }
        else
        {
            writer = new StreamWriter(FilePath, true);
        }

        writer.WriteLine("Response body:");
        writer.WriteLine($"Status Code: {response.StatusCode}");
        writer.WriteLine(response.JsonResponse != null ? response.JsonResponse : response.StringResponse);
        writer.Close();
    }

    public void PrintHeaders(ResponseType response)
    {
        Console.WriteLine("Printing headers to file");
        Debug.Assert(FilePath != null, nameof(FilePath) + " != null");

        StreamWriter writer;
        if (_initialWrite)
        {
            writer = new StreamWriter(FilePath);
            _initialWrite = false;
            writer.WriteLine($"Response, date {DateTime.Now}:");
        }
        else
        {
            writer = new StreamWriter(FilePath, true);
        }

        writer.WriteLine($"Headers: {response.Headers}");
        writer.Close();
    }

    public void PrintException(ResponseType response)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Printing exception info to file {FilePath}");

        Debug.Assert(FilePath != null, nameof(FilePath) + " != null");
        StreamWriter writer;
        if (_initialWrite)
        {
            writer = new StreamWriter(FilePath);
            _initialWrite = false;
            writer.WriteLine($"Response, date {DateTime.Now}:");
        }
        else
        {
            writer = new StreamWriter(FilePath, true);
        }

        writer.WriteLine("An exception occurred during a HTTP Request:");
        writer.WriteLine(response.ErrorMessage);
        writer.Close();
    }
}
