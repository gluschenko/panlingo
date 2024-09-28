#pragma once

#include <stdint.h>
#include <stddef.h>
#include <stdbool.h>

#ifndef EXPORT
#   if defined(_WIN32) || defined(_WIN64)
#       define EXPORT __declspec(dllexport)
#   else
#       define EXPORT extern
#   endif
#endif

extern "C" {

    typedef struct fasttext_t fasttext_t;

    typedef struct {
        float prob;
        char* label;
    } fasttext_prediction_t;

    typedef struct {
        fasttext_prediction_t* predictions;
        size_t length;
    } fasttext_predictions_t;

    typedef struct {
        char** tokens;
        size_t length;
    } fasttext_tokens_t;
    
    typedef struct {
        char** labels;
        int64_t* freqs;
        size_t length;
    } fasttext_labels_t;

    EXPORT void DestroyString(char* s);
    EXPORT fasttext_t* CreateFastText(void);
    EXPORT void DestroyFastText(fasttext_t* handle);
    EXPORT void FastTextLoadModel(fasttext_t* handle, const char* filename, char** err_ptr);
    EXPORT int FastTextGetModelDimensions(fasttext_t* handle);
    EXPORT fasttext_predictions_t* FastTextPredict(fasttext_t* handle, const char* text, int32_t k, float threshold, char** err_ptr);
    EXPORT void DestroyPredictions(fasttext_predictions_t* predictions);
    EXPORT fasttext_labels_t* FastTextGetLabels(fasttext_t* handle);
    EXPORT void DestroyLabels(fasttext_labels_t* labels);
    EXPORT fasttext_tokens_t* FastTextTokenize(fasttext_t* handle, const char* text);
    EXPORT void DestroyTokens(fasttext_tokens_t* tokens);
    EXPORT void FastTextAbort(fasttext_t* handle);
}

struct membuf : std::streambuf {
    membuf(char const* base, size_t size) {
        char* p(const_cast<char*>(base));
        this->setg(p, p, p + size);
    }
};

struct imemstream : virtual membuf, std::istream {
    imemstream(char const* base, size_t size) : membuf(base, size), std::istream(static_cast<std::streambuf*>(this)) {
    }
};

class FastTextExtension : public fasttext::FastText {
public:
    void loadModelData(const char* modelBytes, size_t size) {
        imemstream stream(modelBytes, size);

        if (!this->checkModel(stream)) {
            throw std::invalid_argument("Invalid file format!");
        }

        this->loadModel(stream);
    }
};

