#include <cstddef>
#include <cstring>
#include <vector>
#include <string>
#include "./cld2/public/compact_lang_det.h"
#include "binding.h"

#define MAX_LANGUAGE_COUNT 3

extern "C"
{
    PredictionResult* PredictLanguage(char *text, int* resultCount)
    {
        int textLength = strlen(text);

        bool is_plain_text = true;
        CLD2::CLDHints cldhints = {NULL, NULL, 0, CLD2::UNKNOWN_LANGUAGE};
        bool allow_extended_lang = true;
        int flags = 0;
        CLD2::ResultChunkVector result_chunk_vector;
        int text_bytes;
        bool is_reliable;

        CLD2::Language languages[MAX_LANGUAGE_COUNT];
        int percents[MAX_LANGUAGE_COUNT];
        double scores[MAX_LANGUAGE_COUNT];

        CLD2::Language summary_lang = CLD2::UNKNOWN_LANGUAGE;

        summary_lang = CLD2::ExtDetectLanguageSummary(
            text,
            textLength,
            is_plain_text,
            &cldhints,
            flags,
            languages,
            percents,
            scores,
            &result_chunk_vector,
            &text_bytes,
            &is_reliable);

        int predictionCount = 0;

        for (int i = 0; i < MAX_LANGUAGE_COUNT; ++i)
        {
            if (percents[i] > 0 || (i == 0 && languages[i] == CLD2::UNKNOWN_LANGUAGE))
            {
                predictionCount++;
            }
        }

        *resultCount = predictionCount;

        int scoreTotal = 0;

        for (int i = 0; i < predictionCount; ++i) {
            scoreTotal += scores[i];
        }

        PredictionResult* result = new PredictionResult[predictionCount];
        for (int i = 0; i < predictionCount; ++i) {

            CLD2::Language language = languages[i];
            double probability = scoreTotal > 0 ? scores[i] / (double)scoreTotal : 1.0;
            double proportion = percents[i] / 100.0;

            result[i].language = strdup(CLD2::LanguageCode(language));
            result[i].script = strdup(CLD2::ULScriptCode(CLD2::LanguageRecognizedScript(language, 0)));
            result[i].probability = probability;
            result[i].is_reliable = is_reliable;
            result[i].proportion = proportion;
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