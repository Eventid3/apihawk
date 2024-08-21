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

        var bodyOption = new Option<string>(
            name: "--body",
            description: "Add a custom body for the HTTP POST/PUT request");

        // ----- GET COMMAND -------
        var getCommand = new Command(
            name: "get",
            description: "Sends a HTTP GET requests to the given URL.");

        getCommand.Add(mainUrl);

        getCommand.SetHandler(async (url) =>
            {
                var response = await HttpCaller(HttpRequestType.Get, url);
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

        postCommand.Add(mainUrl);
        postCommand.Add(bodyOption);

        postCommand.SetHandler(async (url, body) =>
            {
                var response = await HttpCaller(HttpRequestType.Post, url, body);

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
                var response = await HttpCaller(HttpRequestType.Delete, url);
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
        var putCommand = new Command(
            name: "put",
            description: "Sends a HTTP PUT request to the given URL.");

        putCommand.Add(mainUrl);
        putCommand.Add(bodyOption);
        putCommand.SetHandler(async (url, body) =>
            {
                var response = await HttpCaller(HttpRequestType.Put, url, body);

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
        // ------ ROOT COMMAND ------
        var rootCommand = new RootCommand("An API Tool for testing API's in development");
        rootCommand.Add(getCommand);
        rootCommand.Add(postCommand);
        rootCommand.Add(deleteCommand);
        rootCommand.Add(putCommand);

        await rootCommand.InvokeAsync(args);

        return 0;
    }


    private enum HttpRequestType
    {
        Get,
        Post,
        Delete,
        Put
    }

    private static async Task<ResponseType> HttpCaller(HttpRequestType request, string url, string? body = null)
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
