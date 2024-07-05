﻿using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD2.Native
{
    public static class CLD2NativeLibrary
    {
        public const string Name = "libcld2.so";

        public static void LoadNativeLibrary()
        {
            NativeLibrary.Load(Name);
        }
    }
}