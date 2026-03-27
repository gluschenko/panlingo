namespace Panlingo.Samples.ViewModels;

public sealed class DetectorOption
{
    public DetectorOption(DetectorKind kind, string displayName, string shortDescription, string description)
    {
        Kind = kind;
        DisplayName = displayName;
        ShortDescription = shortDescription;
        Description = description;
    }

    public DetectorKind Kind { get; }

    public string DisplayName { get; }

    public string ShortDescription { get; }

    public string Description { get; }
}
