using ApiHawk.Core;

namespace ApiHawk.Desktop;

public class MauiResponsePrinter : IResponsePrinter
{
    private Label _outputLabel;

    public MauiResponsePrinter(Label outputLabel)
    {
        _outputLabel = outputLabel;
    }

    public void PrintStandard(ResponseType response)
    {
        var formattedString = new FormattedString();

        var statusCodeSpan = new Span
        {
            Text = $"Status Code: {response.StatusCode}\n\n",
            TextColor = response.StatusCode switch
            {
                >= 200 and <= 299 => Colors.Green,
                >= 300 and <= 399 => Colors.Yellow,
                >= 400 and <= 499 => Colors.Red,
                >= 500 and <= 599 => Colors.Magenta,
                _ => Colors.White
            }
        };

        formattedString.Spans.Add(statusCodeSpan);

        var responseBodySpan = new Span
        {
            Text = response.JsonResponse != null
                ? $"Response Body:\n{response.JsonResponse}"
                : $"Response Body:\n{response.StringResponse}",
            TextColor = Colors.White
        };

        formattedString.Spans.Add(responseBodySpan);

        _outputLabel.FormattedText = formattedString;
    }

    public void PrintHeaders(ResponseType response)
    {
        var formattedString = new FormattedString();

        if (response.Headers != null)
        {
            var headersSpan = new Span
            {
                Text = "Headers:\n",
                TextColor = Colors.Blue
            };
            formattedString.Spans.Add(headersSpan);

            foreach (var c in response.Headers)
            {
                var charSpan = new Span
                {
                    Text = c.ToString(),
                    TextColor = c switch
                    {
                        ':' => Colors.DarkCyan,
                        '\n' => Colors.Blue,
                        _ => Colors.Blue
                    }
                };
                formattedString.Spans.Add(charSpan);
            }
        }

        _outputLabel.FormattedText = formattedString;
    }

    public void PrintException(ResponseType response)
    {
        var formattedString = new FormattedString();

        var exceptionSpan = new Span
        {
            Text = "An exception occurred during a HTTP Request:",
            TextColor = Colors.Red
        };
        formattedString.Spans.Add(exceptionSpan);

        var errorMessageSpan = new Span
        {
            Text = response.ErrorMessage,
            TextColor = Colors.White
        };
        formattedString.Spans.Add(errorMessageSpan);

        _outputLabel.FormattedText = formattedString;
    }
}
