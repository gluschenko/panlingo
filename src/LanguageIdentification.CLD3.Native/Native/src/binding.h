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

    EXPORT void* create_cld3(int minNumBytes, int maxNumBytes);
    EXPORT void destroy_cld3(void* identifier);
    EXPORT PredictionResult cld3_find_language(void* identifier, const char* text);
    EXPORT PredictionResult* cld3_find_languages(void* identifier, const char* text, int numLangs, int* resultCount);
    EXPORT void cld3_destroy_prediction_result(PredictionResult* results, int count);
}