namespace Panlingo.Samples.ViewModels;

public sealed class SampleTextOption
{
    public SampleTextOption(string title, string text)
    {
        Title = title;
        Text = text;
    }

    public string Title { get; }

    public string Text { get; }

    public override string ToString()
    {
        return Title;
    }
}
