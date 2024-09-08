#pragma once

#include "base.h"
#include "nnet_language_identifier.h"

using namespace std;

#ifndef EXPORT
#   ifdef __linux__
#       define EXPORT __attribute__((visibility("default")))
#   else
#       if defined(_MSC_VER)
#           define EXPORT __declspec(dllexport)
#       else
#           define EXPORT __attribute__((visibility("default")))
#       endif
#   endif
#endif

extern "C" {
    struct PredictionResult {
        const char* language;
        double probability;
        bool is_reliable;
        double proportion;
    };

    EXPORT void* CreateIdentifier(int minNumBytes, int maxNumBytes);
    EXPORT void FreeIdentifier(void* identifier);
    EXPORT PredictionResult FindLanguage(void* identifier, const char* text);
    EXPORT PredictionResult* FindLanguages(void* identifier, const char* text, int numLangs, int* resultCount);
    EXPORT void FreeResults(PredictionResult* results, int count);
}