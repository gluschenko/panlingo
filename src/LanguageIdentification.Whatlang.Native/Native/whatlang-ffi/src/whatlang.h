#ifndef WHATLANG_H
#define WHATLANG_H

#include <stddef.h>
#include <stdint.h>
#include <stdbool.h>

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

typedef enum WhatlangStatus {
    WHATLANG_OK = 0,
    WHATLANG_DETECT_FAILURE = 1,
    WHATLANG_BAD_TEXT_PTR = 2,
    WHATLANG_BAD_OUTPUT_PTR = 3,
    WHATLANG_BAD_ENUM_VALUE = 4
} WhatlangStatus;

struct WhatlangPredictionResult {
  uint8_t lang;
  uint8_t script;
  double confidence;
  bool is_reliable;
};

EXPORT WhatlangStatus whatlang_detect(const char* text, size_t text_len, struct WhatlangPredictionResult* result);
EXPORT WhatlangStatus whatlang_detect_script(const char* text, size_t text_len, uint8_t* result);
EXPORT size_t whatlang_lang_code(uint8_t lang, char* result, size_t result_len);
EXPORT size_t whatlang_lang_eng_name(uint8_t lang, char* result, size_t result_len);
EXPORT size_t whatlang_lang_name(uint8_t lang, char* result, size_t result_len);
EXPORT size_t whatlang_script_name(uint8_t script, char* result, size_t result_len);

#ifdef __cplusplus
}
#endif

#endif /* WHATLANG_H */
