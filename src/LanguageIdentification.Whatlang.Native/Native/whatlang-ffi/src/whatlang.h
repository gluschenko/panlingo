#ifndef WHATLANG_H
#define WHATLANG_H

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

typedef enum WhatlangStatus {
    WHATLANG_OK = 0,
    WHATLANG_DETECT_FAILURE = 1,
    WHATLANG_BAD_TEXT_PTR = 2,
    WHATLANG_BAD_OUTPUT_PTR = 3
} LinguaStatus;

typedef enum WhatlangLanguage WhatlangLanguage;
typedef enum WhatlangScript WhatlangScript;

struct WhatlangPredictionResult {
  WhatlangLanguage lang;
  WhatlangScript script;
  double confidence;
  bool is_reliable;
};

EXPORT WhatlangStatus whatlang_detect(const char* text, struct WhatlangPredictionResult* result);
EXPORT WhatlangStatus whatlang_detect_script(const char* text, struct WhatlangScript* result);
EXPORT size_t whatlang_lang_code(WhatlangLanguage lang, char* result);
EXPORT size_t whatlang_lang_eng_name(WhatlangLanguage lang, char* result);
EXPORT size_t whatlang_lang_name(WhatlangLanguage lang, char* result);
EXPORT size_t whatlang_script_name(WhatlangScript script, char* result);

#ifdef __cplusplus
}
#endif

#endif /* WHATLANG_H */