namespace Panlingo.Samples.ViewModels;

public sealed class DetectionResultItem
{
    public DetectionResultItem(string title, string summary, string details)
    {
        Title = title;
        Summary = summary;
        Details = details;
    }

    public string Title { get; }

    public string Summary { get; }

    public string Details { get; }
}
