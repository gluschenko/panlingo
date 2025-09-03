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
build:windows --define=xnn_enable_avx512amx=false
build:windows --define=xnn_enable_avx512fp16=false
build:windows --define=xnn_enable_avxvnni=false
build:windows --define=xnn_enable_avxvnniint8=false
build:windows --cxxopt=/MT
build:windows --host_cxxopt=/MT
build:windows --linkopt=libucrt.lib
build:windows --linkopt=/NODEFAULTLIB:MSVCRT

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

console.log('[Monkey patching is started]');

await a();
await b();
await c();
await d();

console.log('[Monkey patching is done]');

