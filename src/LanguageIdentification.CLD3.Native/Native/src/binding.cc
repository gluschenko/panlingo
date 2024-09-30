#include "binding.h"
#include <cstring>

using namespace chrome_lang_id;

void* create_cld3(int minNumBytes, int maxNumBytes) {
    return new NNetLanguageIdentifier(minNumBytes, maxNumBytes);
}

void destroy_cld3(void* identifier) {
    delete static_cast<NNetLanguageIdentifier*>(identifier);
}

PredictionResult* cld3_find_language(void* identifier, const char* text, int* resultCount) {
    NNetLanguageIdentifier* nativeIdentifier = static_cast<NNetLanguageIdentifier*>(identifier);
    auto nativeResult = nativeIdentifier->FindLanguage(text);

    *resultCount = 1;
    PredictionResult* result = new PredictionResult[*resultCount];
    result[0].language = strdup(nativeResult.language.c_str());
    result[0].probability = nativeResult.probability;
    result[0].is_reliable = nativeResult.is_reliable;
    result[0].proportion = nativeResult.proportion;
    return result;
}

PredictionResult* cld3_find_languages(void* identifier, const char* text, int numLangs, int* resultCount) {
    NNetLanguageIdentifier* nativeIdentifier = static_cast<NNetLanguageIdentifier*>(identifier);
    auto nativeResults = nativeIdentifier->FindTopNMostFreqLangs(text, numLangs);

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

void cld3_destroy_prediction_result(PredictionResult* results, int count) {
    for (int i = 0; i < count; ++i) {
        free((void*)results[i].language);
    }
    delete[] results;
}