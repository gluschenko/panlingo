#ifndef LINGUA_H
#define LINGUA_H

#ifndef EXPORT
#   if defined(_WIN32) || defined(_WIN64)
#       define EXPORT __declspec(dllimport)
#   else
#       define EXPORT extern
#   endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

#include <stddef.h>
#include <stdint.h>

typedef enum LinguaStatus {
    LINGUA_OK = 0,
    LINGUA_DETECT_FAILURE = 1,
    LINGUA_BAD_TEXT_PTR = 2,
    LINGUA_BAD_OUTPUT_PTR = 3,
    LINGUA_BAD_DETECTOR_PTR = 4,
    LINGUA_BAD_ENUM_VALUE = 5
} LinguaStatus;

typedef struct LinguaPredictionResult {
    uint8_t language;
    double confidence;
} LinguaPredictionResult;

typedef struct LinguaPredictionRangeResult {
    uint8_t language;
    double confidence;
    uint32_t start_index;
    uint32_t end_index;
    uint32_t word_count;
} LinguaPredictionRangeResult;

typedef struct LinguaPredictionListResult {
    LinguaPredictionResult* predictions;
    uint32_t predictionsCount;
} LinguaPredictionListResult;

typedef struct LinguaPredictionRangeListResult {
    LinguaPredictionRangeResult* predictions;
    uint32_t predictionsCount;
} LinguaPredictionRangeListResult;

typedef struct LanguageDetector LanguageDetector;
typedef struct LinguaPredictionBuilder LinguaPredictionBuilder;

EXPORT size_t lingua_language_code(uint8_t language, uint8_t code, char* buffer, size_t buffer_len);

EXPORT LinguaPredictionBuilder* lingua_language_detector_builder_create(const uint8_t* languages, size_t language_count);

EXPORT LinguaPredictionBuilder* lingua_language_detector_builder_with_low_accuracy_mode(
    const LinguaPredictionBuilder* builder
);
EXPORT LinguaPredictionBuilder* lingua_language_detector_builder_with_preloaded_language_models(
    const LinguaPredictionBuilder* builder
);
EXPORT LinguaPredictionBuilder* lingua_language_detector_builder_with_minimum_relative_distance(
    const LinguaPredictionBuilder* builder,
    double distance
);

EXPORT LanguageDetector* lingua_language_detector_create(LinguaPredictionBuilder *builder);

EXPORT void lingua_language_detector_builder_destroy(LinguaPredictionBuilder *builder);

EXPORT void lingua_language_detector_destroy(LanguageDetector *detector);

EXPORT void lingua_prediction_result_destroy(LinguaPredictionResult *result, size_t count);

EXPORT void lingua_prediction_range_result_destroy(LinguaPredictionRangeResult *result, size_t count);

EXPORT LinguaStatus lingua_detect_single(
    const LanguageDetector *detector,
    const char *text,
    size_t text_len,
    LinguaPredictionListResult *result
);

EXPORT LinguaStatus lingua_detect_mixed(
    const LanguageDetector *detector,
    const char *text,
    size_t text_len,
    LinguaPredictionRangeListResult *result
);

#ifdef __cplusplus
}
#endif

#endif /* LINGUA_H */
