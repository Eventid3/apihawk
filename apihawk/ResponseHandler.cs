namespace API_Tester;

public class ResponseHandler
{
    private bool Verbose { get; set; }

    private IResponsePrinter _printer;

    public ResponseHandler(bool verbose, IResponsePrinter printer)
    {
        Verbose = verbose;
        _printer = printer;
    }

    public void HandleResponse(ResponseType response)
    {
        if (response)
        {
            if (Verbose)
            {
                _printer.PrintHeaders(response);
            }

            _printer.PrintStandard(response);
        }
        else
        {
            _printer.PrintException(response);
        }
    }
}
