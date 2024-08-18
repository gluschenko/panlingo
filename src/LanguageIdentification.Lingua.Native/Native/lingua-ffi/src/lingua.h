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

typedef struct DetectionResult {
    int language;
    double confidence;
} DetectionResult;

typedef struct LanguageDetectorListResult {
    DetectionResult* predictions;
    size_t predictionsCount;
} LanguageDetectorListResult;

typedef struct LanguageDetector LanguageDetector;
typedef struct LanguageDetectorBuilder LanguageDetectorBuilder;

EXPORT size_t lingua_language_code(size_t language, char* buffer);

EXPORT LanguageDetectorBuilder* lingua_language_detector_builder_create(const int* languages, size_t language_count);

EXPORT LanguageDetector* lingua_language_detector_create(LanguageDetectorBuilder *builder);

EXPORT void lingua_language_detector_builder_destroy(LanguageDetectorBuilder *builder);

EXPORT void lingua_language_detector_destroy(LanguageDetector *detector);

EXPORT LinguaStatus lingua_detect_single(
    const LanguageDetector *detector,
    const char *text,
    DetectionResult *result
);

EXPORT LinguaStatus lingua_detect_multiple(
    const LanguageDetector *detector,
    const char *text,
    DetectionResult *result
);

#ifdef __cplusplus
}
#endif

#endif /* LINGUA_H */