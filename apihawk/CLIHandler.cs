using System.CommandLine;
using System.Diagnostics;
using System.Xml.XPath;
using Newtonsoft.Json;

namespace API_Tester;

public class CLIHandler
{
    // --- Commands ---
    private Command? _rootCommand;
    private Command? _getCommand;
    private Command? _postCommand;
    private Command? _deleteCommand;
    private Command? _putCommand;
    private Command? _patchCommand;
    private Command? _batchCommand;

    // --- Arguments ---
    private Argument<string>? _mainUrl;
    private Argument<string>? _batchFile;

    // --- Options ---
    private Option<string>? _bodyOption;
    private Option<bool> _verboseOption = null!;
    private Option<string> _toFileOption = null!;

    // --- Composition elements ---
    private readonly HttpHandler _httpHandler;
    private readonly ResponseHandler _responseHandler;

    private string[] Args { get; set; }

    public CLIHandler(string[] args)
    {
        SetupArguments();
        SetupOptions();
        SetupCommands();

        var parseResult = _rootCommand?.Parse(args);

        bool verboseResult = parseResult?.GetValueForOption(_verboseOption) ?? false;
        string? logFile = string.Empty;

        try
        {
            logFile = parseResult?.GetValueForOption(_toFileOption!);
        }
        catch
        {
            // Console.WriteLine(e.Message);
        }

        IResponsePrinter printer = string.IsNullOrWhiteSpace(logFile)
            ? new ConsoleResponsePrinter()
            : new FileResponsePrinter(logFile);

        _responseHandler = new ResponseHandler(verboseResult, printer);
        _httpHandler = new HttpHandler();

        Args = args;
    }

    private void SetupCommands()
    {
        // --- GET Command ---
        // --------------------

        _getCommand = new Command(
            name: "get",
            description: "Sends a HTTP GET requests to the given URL.");
        Debug.Assert(_mainUrl != null, nameof(_mainUrl) + " != null");
        _getCommand.Add(_mainUrl);

        _getCommand.SetHandler(async (url) =>
            {
                var response = await _httpHandler.Request(new HttpRequest(HttpRequestType.Get, url));
                _responseHandler.HandleResponse(response);
            },
            symbol: _mainUrl
        );

        // --- POST COMMAND ---
        // --------------------
        _postCommand = new Command(
            name: "post",
            description: "Sends a HTTP POST request to the given URL.");

        _postCommand.Add(_mainUrl);

        Debug.Assert(_bodyOption != null, nameof(_bodyOption) + " != null");
        _postCommand.Add(_bodyOption);

        _postCommand.SetHandler(async (url, body) =>
            {
                var response = await _httpHandler.Request(new HttpRequest(HttpRequestType.Post, url, body));
                _responseHandler.HandleResponse(response);
            },
            symbol1: _mainUrl,
            symbol2: _bodyOption
        );

        // --- DELETE COMMAND ---
        // --------------------

        _deleteCommand = new Command(
            name: "delete",
            description: "Sends a HTTP DELETE request to the given url."
        );

        _deleteCommand.Add(_mainUrl);

        _deleteCommand.SetHandler(async (url) =>
            {
                var response = await _httpHandler.Request(new HttpRequest(HttpRequestType.Delete, url));
                _responseHandler.HandleResponse(response);
            },
            symbol: _mainUrl
        );

        // --- PUT COMMAND ---
        // -------------------

        _putCommand = new Command(
            name: "put",
            description: "Sends a HTTP PUT request to the given URL.");

        _putCommand.Add(_mainUrl);
        _putCommand.Add(_bodyOption);
        _putCommand.SetHandler(async (url, body) =>
            {
                var response = await _httpHandler.Request(new HttpRequest(HttpRequestType.Put, url, body));

                _responseHandler.HandleResponse(response);
            },
            symbol1: _mainUrl,
            symbol2: _bodyOption
        );

        // ------- PATCH COMMAND ------
        _patchCommand = new Command(
            name: "patch",
            description: "Sends a HTTP PATCH request to the given URL.");

        _patchCommand.Add(_mainUrl);
        _patchCommand.Add(_bodyOption);
        _patchCommand.SetHandler(async (url, body) =>
        {
            var response = await _httpHandler.Request(new HttpRequest(HttpRequestType.Patch, url, body));
            _responseHandler.HandleResponse(response);

        },
            symbol1: _mainUrl,
            symbol2: _bodyOption
        );

        // ------ BATCH COMMAND ------
        _batchCommand = new Command(
            name: "batch",
            description: "Makes a batch request described in the given json file");

        Debug.Assert(_batchFile != null, nameof(_batchFile) + " != null");
        _batchCommand.Add(_batchFile);

        _batchCommand.SetHandler(async (file) =>
            {
                using StreamReader sr = new StreamReader(file);
                string json = await sr.ReadToEndAsync();
                List<BatchItem>? batchItems = JsonConvert.DeserializeObject<List<BatchItem>>(json);
                var index = 1;
                if (batchItems != null)
                {
                    foreach (BatchItem batchItem in batchItems)
                    {
                        Console.WriteLine($"Performing request {index} out of {batchItems.Count}.");
                        index += 1;

                        ResponseType response;
                        switch (batchItem.Type)
                        {
                            case HttpRequestType.Get:
                                response = await _httpHandler.Request(new HttpRequest(batchItem.Type, batchItem.Url));
                                break;
                            case HttpRequestType.Post:
                            case HttpRequestType.Delete:
                            case HttpRequestType.Put:
                                response = await _httpHandler.Request(new HttpRequest(batchItem.Type, batchItem.Url, batchItem.Body));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _responseHandler.HandleResponse(response);
                    }
                }
            },
            symbol: _batchFile);

        // ------ ROOT COMMAND ------
        _rootCommand = new RootCommand("An API Tool for testing API's in development");
        _rootCommand.Add(_getCommand);
        _rootCommand.Add(_postCommand);
        _rootCommand.Add(_deleteCommand);
        _rootCommand.Add(_putCommand);
        _rootCommand.Add(_batchCommand);
        _rootCommand.AddGlobalOption(_verboseOption);
        _rootCommand.AddGlobalOption(_toFileOption);
    }

    private void SetupOptions()
    {
        _bodyOption = new Option<string>(
            name: "--body",
            description: "Add a custom body for the HTTP POST/PUT request");

        _verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Enables verbose output",
            getDefaultValue: () => false);

        _toFileOption = new Option<string>(
            name: "--log-file",
            description: "Sends the output to a file instead of the console.");
    }

    private void SetupArguments()
    {
        _mainUrl = new Argument<string>(
            name: "url",
            description: "The URL for the get command");

        _batchFile = new Argument<string>(
            name: "batch file",
            description: "File with the described http requests in JSON format");
    }

    public async Task<int> Initiate()
    {
        Debug.Assert(_rootCommand != null, nameof(_rootCommand) + " != null");
        await _rootCommand.InvokeAsync(Args);

        return 0;
    }
}
