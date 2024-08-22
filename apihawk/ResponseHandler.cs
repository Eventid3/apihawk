namespace API_Tester;

public class ResponseHandler
{
    private bool Verbose { get; set; }
    private string? ToFile { get; set; }

    public ResponseHandler(bool verbose, string? toFile)
    {
        Verbose = verbose;
        ToFile = toFile;
    }

    public void HandleResponse(ResponseType response)
    {
        if (Verbose)
        {
            Console.WriteLine("TODO: Verbose option on.");
        }

        if (ToFile != "")
        {
            Console.WriteLine("TODO: Printing to log file...");
        }
        if (response)
        {
            response.Print();
        }
        else
        {
            response.PrintException();
        }
    }
}
