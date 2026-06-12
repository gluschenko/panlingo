#include "binding.h"

#include <cstring>
#include <string>

using namespace chrome_lang_id;

extern "C" {
    EXPORT void* create_cld3(int minNumBytes, int maxNumBytes) {
        try {
            if (minNumBytes < 0 || maxNumBytes < minNumBytes) {
                return nullptr;
            }

            return new NNetLanguageIdentifier(minNumBytes, maxNumBytes);
        }
        catch (...) {
            return nullptr;
        }
    }

    EXPORT void destroy_cld3(void* identifier) {
        delete static_cast<NNetLanguageIdentifier*>(identifier);
    }

    EXPORT PredictionResult* cld3_find_language(void* identifier, const char* text, size_t textLength, int* resultCount) {
        if (resultCount == nullptr) {
            return nullptr;
        }

        *resultCount = 0;

        if (identifier == nullptr || (text == nullptr && textLength > 0)) {
            return nullptr;
        }

        try {
            NNetLanguageIdentifier* nativeIdentifier = static_cast<NNetLanguageIdentifier*>(identifier);
            std::string input(text == nullptr ? "" : text, textLength);
            auto nativeResult = nativeIdentifier->FindLanguage(input);

            *resultCount = 1;
            PredictionResult* result = new PredictionResult[*resultCount];
            result[0].language = strdup(nativeResult.language.c_str());
            result[0].probability = nativeResult.probability;
            result[0].is_reliable = nativeResult.is_reliable;
            result[0].proportion = nativeResult.proportion;
            return result;
        }
        catch (...) {
            *resultCount = 0;
            return nullptr;
        }
    }

    EXPORT PredictionResult* cld3_find_languages(void* identifier, const char* text, size_t textLength, int numLangs, int* resultCount) {
        if (resultCount == nullptr) {
            return nullptr;
        }

        *resultCount = 0;

        if (identifier == nullptr || numLangs < 0 || (text == nullptr && textLength > 0)) {
            return nullptr;
        }

        try {
            NNetLanguageIdentifier* nativeIdentifier = static_cast<NNetLanguageIdentifier*>(identifier);
            std::string input(text == nullptr ? "" : text, textLength);
            auto nativeResults = nativeIdentifier->FindTopNMostFreqLangs(input, numLangs);

            *resultCount = static_cast<int>(nativeResults.size());
            PredictionResult* result = new PredictionResult[*resultCount];
            for (int i = 0; i < *resultCount; ++i) {
                result[i].language = strdup(nativeResults[i].language.c_str());
                result[i].probability = nativeResults[i].probability;
                result[i].is_reliable = nativeResults[i].is_reliable;
                result[i].proportion = nativeResults[i].proportion;
            }
            return result;
        }
        catch (...) {
            *resultCount = 0;
            return nullptr;
        }
    }

    EXPORT void cld3_destroy_prediction_result(PredictionResult* results, int count) {
        if (results == nullptr) {
            return;
        }

        for (int i = 0; i < count; ++i) {
            free((void*)results[i].language);
        }
        delete[] results;
    }
}
