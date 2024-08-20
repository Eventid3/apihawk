using System.Net.Http.Json;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Text.Json.Nodes;
using API_Tester;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var mainUrl = new Argument<string>(
            name: "url",
            description: "The URL for the get command");

        // ----- GET COMMAND -------
        var getCommand = new Command(
            name: "get",
            description: "Sends a HTTP GET requests to the given URL.");

        getCommand.Add(mainUrl);

        getCommand.SetHandler(async (url) =>
            {
                var response = await HttpCaller(HttpRequestType.GET, url);
                if (response)
                {
                    response.Print();
                }
                else
                {
                    response.PrintException();
                }
            },
            symbol: mainUrl
        );


        // ------ POST COMMAND ------
        var postCommand = new Command(
            name: "post",
            description: "Sends a HTTP POST request to the given URL.");

        var bodyOption = new Option<string>(
            name: "--body",
            description: "Add a custom body for the HTTP POST request");

        postCommand.Add(mainUrl);
        postCommand.Add(bodyOption);

        postCommand.SetHandler(async (url, body) =>
            {
                var response = await HttpCaller(HttpRequestType.POST, url, body);

                if (response)
                {
                    response.Print();
                }
                else
                {
                    response.PrintException();
                }
            },
            symbol1: mainUrl,
            symbol2: bodyOption
        );

        // ------ DELETE COMMAND ------

        var deleteCommand = new Command(
            name: "delete",
            description: "Sends a HTTP DELETE request to the given url."
        );

        deleteCommand.Add(mainUrl);

        deleteCommand.SetHandler(async (url) =>
            {
                var response = await HttpCaller(HttpRequestType.DELETE, url);
                if (response)
                {
                    response.Print();
                }
                else
                {
                    response.PrintException();
                }
            },
            symbol: mainUrl
        );
        // ------ PUT COMMAND ------


        // ------ ROOT COMMAND ------
        var rootCommand = new RootCommand("An API Tool for testing API's in development");
        rootCommand.Add(getCommand);
        rootCommand.Add(postCommand);
        rootCommand.Add(deleteCommand);

        await rootCommand.InvokeAsync(args);

        return 0;
    }


    private enum HttpRequestType
    {
        GET,
        POST,
        DELETE,
        PUT
    }

    private static async Task<ResponseType> HttpCaller(HttpRequestType request, string url, string? body = null)
    {
        var httpClient = new HttpClient();

        try
        {
            HttpResponseMessage? response = null;
            switch (request)
            {
                case HttpRequestType.GET:
                    Console.WriteLine("HttpRequest: GET");
                    response = await httpClient.GetAsync(url);
                    break;
                case HttpRequestType.POST:
                    Console.WriteLine("HttpRequest: POST");
                    if (body != null)
                    {
                        var content = new StringContent(body, Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync(url, content);
                    }

                    break;
                case HttpRequestType.DELETE:
                    Console.WriteLine("HttpRequest: DELETE");
                    response = await httpClient.DeleteAsync(url);
                    break;
                case HttpRequestType.PUT:
                    //TODO
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
