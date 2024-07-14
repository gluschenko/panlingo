namespace LanguageIdentification.Whatlang
{
    public enum WhatLangStatus : byte
    {
        OK = 0,
        DetectFailure = 1,
        BadTextPtr = 2,
        BadOutputPtr = 3,
    }
}
