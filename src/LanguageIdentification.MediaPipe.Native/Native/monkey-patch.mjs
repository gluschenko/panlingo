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
    const newTextA = `build:linux --define=xnn_enable_avx512amx=false\nbuild:linux --define=xnn_enable_avx512fp16=false\nbuild:linux --define=xnn_enable_avxvnniint8=false\n\n`;

    const oldTextB = new RegExp(`build:windows --host_copt=/D_USE_MATH_DEFINES${lineEnding}`, 'g');
    const newTextB = `build:windows --host_copt=/D_USE_MATH_DEFINES\nbuild:windows --define=xnn_enable_avx512amx=false\nbuild:windows --define=xnn_enable_avx512fp16=false\nbuild:windows --define=xnn_enable_avxvnni=falsee\nbuild:windows --define=xnn_enable_avxvnniint8=false\n\n`;

    const oldTextC = new RegExp(`c\\+\\+17`, 'g');
    const newTextC = `c++20`;

    findAndPatch(".bazelrc", [
        { a: oldTextA, b: newTextA },
        { a: oldTextB, b: newTextB },
        { a: oldTextC, b: newTextC },
    ]);
}

console.log('[Monkey patching is started]');

await a();
await b();

console.log('[Monkey patching is done]');

