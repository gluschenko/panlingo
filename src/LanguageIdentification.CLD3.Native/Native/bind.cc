#include "bind.h"
#include <cstring>

using namespace chrome_lang_id;

void* CreateIdentifier(int minNumBytes, int maxNumBytes) {
    return new NNetLanguageIdentifier(minNumBytes, maxNumBytes);
}

void FreeIdentifier(void* identifier) {
    delete static_cast<NNetLanguageIdentifier*>(identifier);
}

Result FindLanguage(void* identifier, const char* text) {
    NNetLanguageIdentifier* nativeIdentifier = static_cast<NNetLanguageIdentifier*>(identifier);
    auto nativeResult = nativeIdentifier->FindLanguage(text);

    Result result;
    result.language = strdup(nativeResult.language.c_str());
    result.probability = nativeResult.probability;
    result.is_reliable = nativeResult.is_reliable;
    result.proportion = nativeResult.proportion;
    return result;
}

Result* FindTopNMostFreqLangs(void* identifier, const char* text, int numLangs, int* resultCount) {
    NNetLanguageIdentifier* nativeIdentifier = static_cast<NNetLanguageIdentifier*>(identifier);
    auto nativeResults = nativeIdentifier->FindTopNMostFreqLangs(text, numLangs);

    *resultCount = static_cast<int>(nativeResults.size());
    Result* results = new Result[*resultCount];
    for (int i = 0; i < *resultCount; ++i) {
        results[i].language = strdup(nativeResults[i].language.c_str());
        results[i].probability = nativeResults[i].probability;
        results[i].is_reliable = nativeResults[i].is_reliable;
        results[i].proportion = nativeResults[i].proportion;
    }
    return results;
}

void FreeResults(Result* results, int count) {
    for (int i = 0; i < count; ++i) {
        free((void*)results[i].language);
    }
    delete[] results;
}