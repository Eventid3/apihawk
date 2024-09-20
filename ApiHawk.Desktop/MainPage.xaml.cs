using ApiHawk.Core;

namespace ApiHawk.Desktop;

public partial class MainPage : ContentPage
{
    HttpHandler _httpHandler = new();
    public MainPage()
    {
        InitializeComponent();
    }

    private async void GetButton_OnClicked(object? sender, EventArgs e)
    {
        var url = "https://example.com";
        var request = new HttpRequest(HttpRequestType.Get, url);
        var response = await _httpHandler.Request(request);
        Console.WriteLine($"GET Request to {url} returned {response.StatusCode}");

        if (response.StringResponse != null) ResponseLabel.Text = response.StringResponse;
    }
}
