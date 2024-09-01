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

    void DestroyString(char* s) {
        if (s != nullptr) {
            free(s);
        }
    }

    fasttext_t* CreateFastText(void) {
        return (fasttext_t*)(new FastText());
    }

    void DestroyFastText(fasttext_t* handle) {
        FastText* x = (FastText*)handle;
        delete x;
    }

    void FastTextLoadModel(fasttext_t* handle, const char* filename, char** err_ptr) {
        try {
            ((FastText*)handle)->loadModel(filename);
        } catch (const std::invalid_argument& e) {
            save_error(err_ptr, e);
        }
    }

    int FastTextGetModelDimensions(fasttext_t* handle) {
        return ((FastText*)handle)->getDimension();
    }

    fasttext_predictions_t* FastTextPredict(fasttext_t* handle, const char* text, int32_t k, float threshold, char** err_ptr) {
        std::vector<std::pair<fasttext::real, std::string>> predictions;
        std::stringstream ioss(text);
        try {
            ((FastText*)handle)->predictLine(ioss, predictions, k, threshold);
        } catch (const std::invalid_argument& e) {
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

    void DestroyPredictions(fasttext_predictions_t* predictions) {
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

    fasttext_labels_t* FastTextGetLabels(fasttext_t* handle) {
        std::shared_ptr<const fasttext::Dictionary> d = ((FastText*)handle)->getDictionary();
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

    void DestroyLabels(fasttext_labels_t* labels) {
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

    void FastTextAbort(fasttext_t* handle) {
        ((FastText*)handle)->abort();
    }

    fasttext_tokens_t* FastTextTokenize(fasttext_t* handle, const char* text) {
        std::vector<std::string> text_split;
        std::shared_ptr<const fasttext::Dictionary> d = ((FastText*)handle)->getDictionary();
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

    void DestroyTokens(fasttext_tokens_t* tokens) {
        for (size_t i = 0; i < tokens->length; i++) {
            free(tokens->tokens[i]);
        }
        free(tokens->tokens);
        free(tokens);
    }
}
