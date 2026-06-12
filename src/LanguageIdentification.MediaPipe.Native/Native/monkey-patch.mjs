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

function f() {
    const oldText = "b3a24de97a8fdbc835b9833169501030b8977031bcb54b3b3ac13740f846ab30";
    const newText = "ff4ab312d6cc18542ea01941f1fc450ee1c0150240d6cab183ca9bb36e5eaf1a";

    findAndPatch("WORKSPACE", [
        { a: oldText, b: newText },
    ]);
}

console.log('[Monkey patching is started]');

await a();
await b();
await c();
await d();
await e();
await f();

console.log('[Monkey patching is done]');

