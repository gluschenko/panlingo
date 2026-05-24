#!/usr/bin/env zx

async function findAndPatch(endsWith, changes) {
    const files = fs.readdirSync(".", { recursive: true });

    for (const file of files.filter(x => x.endsWith(endsWith))) {
        console.log("Found: " + file);

        const content = await fs.readFile(file, 'utf-8');
        let newContent = content;

        for (const { a, b } of changes) {
            newContent = newContent.replace(a, b);
        }

        if (content !== newContent) {
            await fs.writeFile(file, newContent);
            console.log("Patched: " + file + "\nContent:\n" + newContent);
        }
        else {
            console.log("Nothing to patch: " + file);
        }
    }
}

function a()
{
    const oldText = '\{LATEST_METADATA_PARSER_VERSION\}';
    const newText = '1.5.0';

    findAndPatch(".h.template", [
        { a: oldText, b: newText },
    ]);
}

function b()
{
    const lineEnding = "\\r?\\n";

    const oldTextA = new RegExp(`build:linux --define=xnn_enable_avx512amx=false${lineEnding}`, 'g');
    const newTextA = `
build:linux --define=xnn_enable_avx512amx=false
build:linux --define=xnn_enable_avx512fp16=false
build:linux --define=xnn_enable_avxvnni=false
build:linux --define=xnn_enable_avxvnniint8=false

    `;

    const oldTextB = new RegExp(`build:windows --host_copt=/D_USE_MATH_DEFINES${lineEnding}`, 'g');
    const newTextB = `
build:windows --host_copt=/D_USE_MATH_DEFINES
build:windows --define=xnn_enable_avx512amx=false
build:windows --define=xnn_enable_avx512fp16=false
build:windows --define=xnn_enable_avxvnni=false
build:windows --define=xnn_enable_avxvnniint8=false
build:windows --copt=/MT
build:windows --cxxopt=/MT
build:windows --host_copt=/MT
build:windows --host_cxxopt=/MT
build:windows --linkopt=libcmt.lib
build:windows --linkopt=libvcruntime.lib
build:windows --linkopt=libucrt.lib
build:windows --linkopt=/NODEFAULTLIB:msvcrt.lib
build:windows --linkopt=/NODEFAULTLIB:ucrtbase.lib
build:windows --define=absl_disable_stacktrace=1
build:windows --define=absl_disable_symbolize=1

    `;

    findAndPatch(".bazelrc", [
        { a: oldTextA, b: newTextA },
        { a: oldTextB, b: newTextB },
    ]);
}

// MSVC:
// error C7555: use of designated initializers requires at least '/std:c++20'
function c() {
    const lineEnding = "\\r?\\n";

    const oldText = new RegExp(
        `    language_detector_result\\.push_back\\(${lineEnding}` +
        `        \\{\\.language_code = \\*category\\.category_name,${lineEnding}` +
        `         \\.probability = category\\.score\\}\\);${lineEnding}`, 'g');

    const newText = `
    LanguageDetectorPrediction prediction;
    prediction.language_code = *category.category_name;
    prediction.probability = category.score;
    language_detector_result.push_back(prediction);
    `;

    findAndPatch("language_detector.cc", [
        { a: oldText, b: newText },
    ]);
}

// MP_EXPORT with Windows-specific syntax for FFI
function d() {
    const oldText = `#define MP_EXPORT __attribute__((visibility("default")))`;
    const newText = `
#   if defined(_WIN32) || defined(_WIN64)
#       define MP_EXPORT __declspec(dllexport)
#   elif defined(__GNUC__) || defined(__clang__)
#       define MP_EXPORT __attribute__((visibility("default")))
#   else
#       define MP_EXPORT
#   endif
    `;

    findAndPatch("language_detector.h", [
        { a: oldText, b: newText },
    ]);
}

function e() {
    const oldText = "ad37707084a6d4ff41be10cbe8540c75bea057ba79d0de6c367c1bfac6ba0852";
    const newText = "8eeb81ff6bc7ab2de678c0c4a3d18b02c382a5122ac4edc26a3334c858531739";

    findAndPatch("WORKSPACE", [
        { a: oldText, b: newText },
    ]);
}

// Keep the C ABI safe for managed callers: byte+length input, null checks, and string cleanup.
function f() {
    const lineEnding = "\\r?\\n";

    const headerIncludeOld = new RegExp(`#define MEDIAPIPE_TASKS_C_TEXT_LANGUAGE_DETECTOR_LANGUAGE_DETECTOR_H_${lineEnding}${lineEnding}`, 'g');
    const headerIncludeNew = `#define MEDIAPIPE_TASKS_C_TEXT_LANGUAGE_DETECTOR_LANGUAGE_DETECTOR_H_

#include <stddef.h>

`;

    const detectHeaderOld = new RegExp(
        `MP_EXPORT int language_detector_detect\\(void\\* detector, const char\\* utf8_str,${lineEnding}` +
        `                                       struct LanguageDetectorResult\\* result,${lineEnding}` +
        `                                       char\\*\\* error_msg\\);`, 'g');
    const detectHeaderNew = `MP_EXPORT int language_detector_detect(void* detector, const char* utf8_str,
                                       size_t utf8_str_len,
                                       struct LanguageDetectorResult* result,
                                       char** error_msg);`;

    const closeHeaderOld = `MP_EXPORT int language_detector_close(void* detector, char** error_msg);`;
    const closeHeaderNew = `MP_EXPORT int language_detector_close(void* detector, char** error_msg);

// Frees strings allocated by this C API.
MP_EXPORT void language_detector_destroy_string(char* value);`;

    findAndPatch("language_detector.h", [
        { a: headerIncludeOld, b: headerIncludeNew },
        { a: detectHeaderOld, b: detectHeaderNew },
        { a: closeHeaderOld, b: closeHeaderNew },
    ]);
}

function g() {
    const lineEnding = "\\r?\\n";

    const includeOld = new RegExp(`#include <memory>${lineEnding}#include <utility>`, 'g');
    const includeNew = `#include <memory>
#include <string>
#include <utility>`;

    const detectImplOld = new RegExp(
        `int CppLanguageDetectorDetect\\(void\\* detector, const char\\* utf8_str,${lineEnding}` +
        `                              LanguageDetectorResult\\* result,${lineEnding}` +
        `                              char\\*\\* error_msg\\) \\{${lineEnding}` +
        `  auto cpp_detector = static_cast<LanguageDetector\\*>\\(detector\\);${lineEnding}` +
        `  auto cpp_result = cpp_detector->Detect\\(utf8_str\\);`, 'g');
    const detectImplNew = `int CppLanguageDetectorDetect(void* detector, const char* utf8_str,
                              size_t utf8_str_len,
                              LanguageDetectorResult* result,
                              char** error_msg) {
  if (detector == nullptr || result == nullptr ||
      (utf8_str == nullptr && utf8_str_len > 0)) {
    return CppProcessError(absl::InvalidArgumentError("Invalid language detector arguments"), error_msg);
  }

  auto cpp_detector = static_cast<LanguageDetector*>(detector);
  auto input = std::string(utf8_str == nullptr ? "" : utf8_str, utf8_str_len);
  auto cpp_result = cpp_detector->Detect(input);`;

    const closeResultOld = new RegExp(
        `void CppLanguageDetectorCloseResult\\(LanguageDetectorResult\\* result\\) \\{${lineEnding}` +
        `  CppCloseLanguageDetectorResult\\(result\\);${lineEnding}` +
        `\\}`, 'g');
    const closeResultNew = `void CppLanguageDetectorCloseResult(LanguageDetectorResult* result) {
  if (result == nullptr) {
    return;
  }
  CppCloseLanguageDetectorResult(result);
}`;

    const closeOld = new RegExp(
        `int CppLanguageDetectorClose\\(void\\* detector, char\\*\\* error_msg\\) \\{${lineEnding}` +
        `  auto cpp_detector = static_cast<LanguageDetector\\*>\\(detector\\);`, 'g');
    const closeNew = `int CppLanguageDetectorClose(void* detector, char** error_msg) {
  if (detector == nullptr) {
    return 0;
  }

  auto cpp_detector = static_cast<LanguageDetector*>(detector);`;

    const exportedDetectOld = new RegExp(
        `int language_detector_detect\\(void\\* detector, const char\\* utf8_str,${lineEnding}` +
        `                             LanguageDetectorResult\\* result, char\\*\\* error_msg\\) \\{${lineEnding}` +
        `  return mediapipe::tasks::c::text::language_detector::${lineEnding}` +
        `      CppLanguageDetectorDetect\\(detector, utf8_str, result, error_msg\\);${lineEnding}` +
        `\\}`, 'g');
    const exportedDetectNew = `int language_detector_detect(void* detector, const char* utf8_str,
                             size_t utf8_str_len,
                             LanguageDetectorResult* result, char** error_msg) {
  return mediapipe::tasks::c::text::language_detector::
      CppLanguageDetectorDetect(detector, utf8_str, utf8_str_len, result, error_msg);
}`;

    const destroyOld = new RegExp(
        `int language_detector_close\\(void\\* detector, char\\*\\* error_ms\\) \\{${lineEnding}` +
        `  return mediapipe::tasks::c::text::language_detector::CppLanguageDetectorClose\\(${lineEnding}` +
        `      detector, error_ms\\);${lineEnding}` +
        `\\}${lineEnding}${lineEnding}` +
        `\\}  // extern "C"`, 'g');
    const destroyNew = `int language_detector_close(void* detector, char** error_ms) {
  return mediapipe::tasks::c::text::language_detector::CppLanguageDetectorClose(
      detector, error_ms);
}

void language_detector_destroy_string(char* value) {
  free(value);
}

}  // extern "C"`;

    findAndPatch("language_detector.cc", [
        { a: includeOld, b: includeNew },
        { a: detectImplOld, b: detectImplNew },
        { a: closeResultOld, b: closeResultNew },
        { a: closeOld, b: closeNew },
        { a: exportedDetectOld, b: exportedDetectNew },
        { a: destroyOld, b: destroyNew },
    ]);
}

function h() {
    const lineEnding = "\\r?\\n";

    const oldText = new RegExp(
        `void CppCloseLanguageDetectorResult\\(LanguageDetectorResult\\* in\\) \\{${lineEnding}` +
        `  for \\(uint32_t i = 0; i < in->predictions_count; \\+\\+i\\) \\{`, 'g');
    const newText = `void CppCloseLanguageDetectorResult(LanguageDetectorResult* in) {
  if (in == nullptr || in->predictions == nullptr) {
    return;
  }

  for (uint32_t i = 0; i < in->predictions_count; ++i) {`;

    findAndPatch("language_detector_result_converter.cc", [
        { a: oldText, b: newText },
    ]);
}

function i() {
    const lineEnding = "\\r?\\n";

    const includeOld = new RegExp(`#include <cstdlib>${lineEnding}#include <string>`, 'g');
    const includeNew = `#include <cstring>
#include <cstdlib>
#include <string>`;

    const detectOld = `language_detector_detect(detector, kTestString, &result,`;
    const detectNew = `language_detector_detect(detector, kTestString, strlen(kTestString), &result,`;

    findAndPatch("language_detector_test.cc", [
        { a: includeOld, b: includeNew },
        { a: detectOld, b: detectNew },
    ]);
}

console.log('[Monkey patching is started]');

await a();
await b();
await c();
await d();
await e();
await f();
await g();
await h();
await i();

console.log('[Monkey patching is done]');

