using ApiHawk.Core;

namespace ApiHawk.Desktop;

public partial class MainPage : ContentPage
{
    private readonly HttpHandler _httpHandler;
    private readonly IResponsePrinter _printer;

    public MainPage()
    {
        InitializeComponent();
        _httpHandler = new HttpHandler();
        _printer = new MauiResponsePrinter(ResponseLabel);

        RequestTypePicker.ItemsSource = Enum.GetValues<HttpRequestType>();
        RequestTypePicker.SelectedItem = HttpRequestType.Get;
        RequestBodyEditor.Keyboard = Keyboard.Create(KeyboardFlags.None);
        RequestBodyEditor.TextChanged += (s, e) =>
        {
            RequestBodyEditor.Text = RequestBodyEditor.Text.Replace("“", "\"").Replace("”", "\"");
        };
    }


    private async void GoButton_OnClicked(object? sender, EventArgs e)
    {
        ResponseLabel.Text = "";

        var url = UrlEntry.Text;
        var requestType = (HttpRequestType) RequestTypePicker.SelectedItem;
        var body = RequestBodyEditor.Text;

        var request = new HttpRequest(requestType, url, body);
        var response = await _httpHandler.Request(request);
        Console.WriteLine($"GET Request to {url} returned {response.StatusCode}");

        var responseHandler = new ResponseHandler(false, _printer);
        responseHandler.HandleResponse(response);

    }

    private void ClearButton_OnClicked(object? sender, EventArgs e)
    {
        ResponseLabel.Text = "";
    }
}
