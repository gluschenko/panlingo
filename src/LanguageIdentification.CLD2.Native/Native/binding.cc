#include <cstddef>
#include <cstring>
#include <vector>
#include <string>
#include "./cld2/public/compact_lang_det.h"
#include "binding.h"

#define MAX_LANGUAGE_COUNT 3

extern "C"
{
    PredictionResult* PredictLanguage(char *data, int length, int* resultCount)
    {
        bool is_plain_text = true;
        CLD2::CLDHints cldhints = {NULL, NULL, 0, CLD2::UNKNOWN_LANGUAGE};
        bool allow_extended_lang = true;
        int flags = 0;
        CLD2::Language language3[MAX_LANGUAGE_COUNT];
        int percent3[MAX_LANGUAGE_COUNT];
        double normalized_score3[MAX_LANGUAGE_COUNT];
        CLD2::ResultChunkVector result_chunk_vector;
        int text_bytes;
        bool is_reliable;

        if (length <= 0)
        {
            length = strlen(data);
        }

        CLD2::Language summary_lang = CLD2::UNKNOWN_LANGUAGE;

        summary_lang = CLD2::ExtDetectLanguageSummary(
            data,
            length,
            is_plain_text,
            &cldhints,
            flags,
            language3,
            percent3,
            normalized_score3,
            &result_chunk_vector,
            &text_bytes,
            &is_reliable);

        int a = 0;

        for (int i = 0; i < MAX_LANGUAGE_COUNT; ++i)
        {
            // if (percent3[i] > 0) 
            {
                a++;
            }
        }

        *resultCount = a;

        PredictionResult* result = new PredictionResult[*resultCount];
        for (int i = 0; i < *resultCount; ++i) {
            result[i].language = strdup(CLD2::LanguageCode(language3[i]));
            result[i].script = strdup(CLD2::ULScriptCode(CLD2::LanguageRecognizedScript(language3[i], 0)));
            result[i].probability = normalized_score3[i];
            result[i].is_reliable = is_reliable;
            result[i].proportion = percent3[i];
        }

        return result;
    }

    void FreeResults(PredictionResult* results, int count)
    {
        for (int i = 0; i < count; ++i) {
            free((void*)results[i].language);
            free((void*)results[i].script);
        }
        delete[] results;
    }
}