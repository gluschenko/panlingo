#include <stdint.h>
#include <stdlib.h>

struct detection_result {
  int language;
  double confidence;
};

int lingua_detect_single(const char* text, size_t len, struct detection_result* info);


