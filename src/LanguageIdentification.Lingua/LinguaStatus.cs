namespace Panlingo.LanguageIdentification.Lingua
{
    public enum LinguaStatus : byte
    {
        OK = 0,
        DetectFailure = 1,
        BadTextPtr = 2,
        BadOutputPtr = 3,
        BadDetectorPtr = 4,
        BadEnumValue = 5,
    }
}
