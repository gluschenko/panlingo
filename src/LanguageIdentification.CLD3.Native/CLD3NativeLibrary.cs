﻿using System.Runtime.InteropServices;

namespace LanguageIdentification.CLD3.Native
{
    public static class CLD3NativeLibrary
    {
        public const string Name = "libcld3.so";

        public static void LoadNativeLibrary()
        {
            NativeLibrary.Load(Name);
        }
    }
}
