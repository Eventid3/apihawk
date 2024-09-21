using System.Diagnostics;
using ApiHawk.Core;

namespace ApiHawk.CLI;

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
