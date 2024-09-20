using System.Net;

namespace ApiHawk.Core;

public interface IResponsePrinter
{
    void PrintStandard(ResponseType response);
    void PrintHeaders(ResponseType response);
    void PrintException(ResponseType response);
}
