#include <iostream>
#include <sstream>
#include <string.h>

#include "args.h"
#include "fasttext.h"
#include "autotune.h"

#include "binding.h"

using namespace std;
using namespace fasttext;

extern "C" {

    static void save_error(char** err_ptr, const std::exception& e) {
        assert(err_ptr != nullptr);
        *err_ptr = strdup(e.what());
    }

    EXPORT void destroy_string(char* s) {
        if (s != nullptr) {
            free(s);
        }
    }

    EXPORT fasttext_t* create_fasttext(void) {
        return (fasttext_t*)(new FastTextExtension());
    }

    EXPORT void destroy_fasttext(fasttext_t* handle) {
        FastTextExtension* x = (FastTextExtension*)handle;
        delete x;
    }

    EXPORT void fasttext_load_model(fasttext_t* handle, const char* filename, char** err_ptr) {
        try {
            ((FastTextExtension*)handle)->loadModel(filename);
        }
        catch (const std::invalid_argument& e) {
            save_error(err_ptr, e);
        }
    }

    EXPORT void fasttext_load_model_data(fasttext_t* handle, const char* buffer, size_t buffer_length, char** err_ptr) {
        try {
            ((FastTextExtension*)handle)->loadModelData(buffer, buffer_length);
        }
        catch (const std::invalid_argument& e) {
            save_error(err_ptr, e);
        }
    }

    EXPORT int fasttext_get_model_dimensions(fasttext_t* handle) {
        return ((FastTextExtension*)handle)->getDimension();
    }

    EXPORT fasttext_predictions_t* fasttext_predict(fasttext_t* handle, const char* text, int32_t k, float threshold, char** err_ptr) {
        std::vector<std::pair<fasttext::real, std::string>> predictions;
        std::stringstream ioss(text);
        try {
            ((FastTextExtension*)handle)->predictLine(ioss, predictions, k, threshold);
        }
        catch (const std::invalid_argument& e) {
            save_error(err_ptr, e);
            return nullptr;
        }
        size_t len = predictions.size();
        fasttext_predictions_t* ret = static_cast<fasttext_predictions_t*>(malloc(sizeof(fasttext_predictions_t)));
        ret->length = len;
        fasttext_prediction_t* c_preds = static_cast<fasttext_prediction_t*>(malloc(sizeof(fasttext_prediction_t) * len));
        for (size_t i = 0; i < len; i++) {
            c_preds[i].label = strdup(predictions[i].second.c_str());
            c_preds[i].prob = predictions[i].first;
        }
        ret->predictions = c_preds;
        return ret;
    }

    EXPORT void destroy_predictions(fasttext_predictions_t* predictions) {
        if (predictions == nullptr) {
            return;
        }
        for (size_t i = 0; i < predictions->length; i++) {
            fasttext_prediction_t pred = predictions->predictions[i];
            free(pred.label);
        }
        free(predictions->predictions);
        free(predictions);
    }

    EXPORT fasttext_labels_t* fasttext_get_labels(fasttext_t* handle) {
        std::shared_ptr<const fasttext::Dictionary> d = ((FastTextExtension*)handle)->getDictionary();
        std::vector<int64_t> labels_freq = d->getCounts(fasttext::entry_type::label);
        size_t len = labels_freq.size();

        fasttext_labels_t* ret = static_cast<fasttext_labels_t*>(malloc(sizeof(fasttext_labels_t)));
        ret->length = len;
        char** labels = static_cast<char**>(malloc(sizeof(char*) * len));
        int64_t* freqs = static_cast<int64_t*>(malloc(sizeof(int64_t) * len));
        for (int32_t i = 0; i < labels_freq.size(); i++) {
            std::string label = d->getLabel(i);
            labels[i] = strdup(label.c_str());
            freqs[i] = labels_freq[i];
        }
        ret->labels = labels;
        ret->freqs = freqs;
        return ret;
    }

    EXPORT void destroy_labels(fasttext_labels_t* labels) {
        if (labels == nullptr) {
            return;
        }
        for (size_t i = 0; i < labels->length; i++) {
            free(labels->labels[i]);
        }
        free(labels->labels);
        free(labels->freqs);
        free(labels);
    }

    EXPORT fasttext_tokens_t* fasttext_tokenize(fasttext_t* handle, const char* text) {
        std::vector<std::string> text_split;
        std::shared_ptr<const fasttext::Dictionary> d = ((FastTextExtension*)handle)->getDictionary();
        std::stringstream ioss(text);
        std::string token;
        while (!ioss.eof()) {
            while (d->readWord(ioss, token)) {
                text_split.push_back(token);
            }
        }
        size_t len = text_split.size();
        fasttext_tokens_t* ret = static_cast<fasttext_tokens_t*>(malloc(sizeof(fasttext_tokens_t)));
        ret->length = len;
        char** tokens = static_cast<char**>(malloc(sizeof(char*) * len));
        for (size_t i = 0; i < len; i++) {
            tokens[i] = strdup(text_split[i].c_str());
        }
        ret->tokens = tokens;
        return ret;
    }

    EXPORT void destroy_tokens(fasttext_tokens_t* tokens) {
        for (size_t i = 0; i < tokens->length; i++) {
            free(tokens->tokens[i]);
        }
        free(tokens->tokens);
        free(tokens);
    }

    EXPORT void fasttext_abort(fasttext_t* handle) {
        ((FastTextExtension*)handle)->abort();
    }
}
