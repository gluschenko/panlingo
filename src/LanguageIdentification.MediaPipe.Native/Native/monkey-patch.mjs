#!/usr/bin/env zx

async function findAndPatch(endsWith, oldText, newText) {
    const files = fs.readdirSync(".", { recursive: true });

    for (const file of files.filter(x => x.endsWith(endsWith))) {
        console.log("Found: " + file);

        const content = await fs.readFile(file, 'utf-8');
        const newContent = content.replace(oldText, newText);

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

    findAndPatch(".h.template", oldText, newText);
}

function b()
{
    const lineEnding = "\\r?\\n";

    const oldText = new RegExp(`build:linux --define=xnn_enable_avx512amx=false${lineEnding}${lineEnding}`, 'g');
    const newText = `build:linux --define=xnn_enable_avx512amx=false\nbuild:linux --define=xnn_enable_avx512fp16=false\nbuild:linux --define=xnn_enable_avxvnniint8=false\n\n`;

    findAndPatch(".bazelrc", oldText, newText);
}

function c() {
    const lineEnding = "\\r?\\n";

    const oldText = new RegExp(`build:windows --host_copt=/D_USE_MATH_DEFINES${lineEnding}${lineEnding}`, 'g');
    const newText = `build:windows --host_copt=/D_USE_MATH_DEFINES\nbuild:windows --define=xnn_enable_avx512amx=false\nbuild:windows --define=xnn_enable_avx512fp16=false\nbuild:windows --define=xnn_enable_avxvnni=falsee\nbuild:windows --define=xnn_enable_avxvnniint8=false\n\n`;

    findAndPatch(".bazelrc", oldText, newText);
}

function d() {
    const oldText = new RegExp(`-std=c++17`, 'g');
    const newText = `-std=c++20`;

    findAndPatch(".bazelrc", oldText, newText);
}

console.log('[Monkey patching is started]');

await a();
await b();
await c();
await d();

console.log('[Monkey patching is done]');

