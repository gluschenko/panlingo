namespace Panlingo.LanguageIdentification.Whatlang
{
    public enum WhatlangStatus : byte
    {
        OK = 0,
        DetectFailure = 1,
        BadTextPtr = 2,
        BadOutputPtr = 3,
    }
}
