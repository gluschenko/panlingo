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

typedef enum LinguaStatus {
    LINGUA_OK = 0,
    LINGUA_DETECT_FAILURE = 1,
    LINGUA_BAD_TEXT_PTR = 2,
    LINGUA_BAD_OUTPUT_PTR = 3
} LinguaStatus;

typedef enum LinguaLanguage LinguaLanguage;

typedef struct LinguaPredictionResult {
    LinguaLanguage language;
    double confidence;
} LinguaPredictionResult;

typedef struct LinguaPredictionListResult {
    LinguaPredictionResult* predictions;
    size_t predictionsCount;
} LinguaPredictionListResult;

typedef struct LinguaPrediction LinguaPrediction;
typedef struct LinguaPredictionBuilder LinguaPredictionBuilder;

EXPORT size_t lingua_language_code(size_t language, char* buffer);

EXPORT LinguaPredictionBuilder* lingua_language_detector_builder_create(const int* languages, size_t language_count);

EXPORT LinguaPrediction* lingua_language_detector_create(LinguaPredictionBuilder *builder);

EXPORT void lingua_language_detector_builder_destroy(LinguaPredictionBuilder *builder);

EXPORT void lingua_language_detector_destroy(LinguaPrediction *detector);

EXPORT LinguaStatus lingua_detect_single(
    const LinguaPrediction *detector,
    const char *text,
    LinguaPredictionResult *result
);

EXPORT LinguaStatus lingua_detect_multiple(
    const LinguaPrediction *detector,
    const char *text,
    LinguaPredictionResult *result
);

#ifdef __cplusplus
}
#endif

#endif /* LINGUA_H */