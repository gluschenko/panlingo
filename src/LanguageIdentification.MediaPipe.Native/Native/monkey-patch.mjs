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
    const oldText = 'build:linux --define=xnn_enable_avx512amx=false\n\n';
    const newText = 'build:linux --define=xnn_enable_avx512amx=false\nbuild:linux --define=xnn_enable_avx512fp16=false\n\n';

    findAndPatch(".bazelrc", oldText, newText);
}

console.log('[Monkey patching is started]');

await a();
await b();

console.log('[Monkey patching is done]');

