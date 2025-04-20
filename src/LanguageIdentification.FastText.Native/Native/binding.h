#pragma once

#include <stdint.h>
#include <stddef.h>
#include <stdbool.h>

#ifndef EXPORT
#   if defined(_WIN32) || defined(_WIN64)
#       define EXPORT __declspec(dllexport)
#   elif defined(__GNUC__) || defined(__clang__)
#       define EXPORT __attribute__((visibility("default")))
#   else
#       define EXPORT
#   endif
#endif

extern "C" 
{
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

    EXPORT void destroy_string(char* s);
    EXPORT fasttext_t* create_fasttext(void);
    EXPORT void destroy_fasttext(fasttext_t* handle);
    EXPORT void fasttext_load_model(fasttext_t* handle, const char* filename, char** err_ptr);
    EXPORT void fasttext_load_model_data(fasttext_t* handle, const char* buffer, size_t buffer_length, char** err_ptr);
    EXPORT int fasttext_get_model_dimensions(fasttext_t* handle);
    EXPORT fasttext_predictions_t* fasttext_predict(fasttext_t* handle, const char* text, int32_t k, float threshold, char** err_ptr);
    EXPORT void destroy_predictions(fasttext_predictions_t* predictions);
    EXPORT fasttext_labels_t* fasttext_get_labels(fasttext_t* handle);
    EXPORT void destroy_labels(fasttext_labels_t* labels);
    EXPORT fasttext_tokens_t* fasttext_tokenize(fasttext_t* handle, const char* text);
    EXPORT void destroy_tokens(fasttext_tokens_t* tokens);
    EXPORT void fasttext_abort(fasttext_t* handle);
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

